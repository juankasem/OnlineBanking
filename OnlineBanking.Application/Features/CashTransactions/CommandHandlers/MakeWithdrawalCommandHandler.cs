using MediatR;
using OnlineBanking.Application.Common.Helpers;
using OnlineBanking.Application.Contracts.Infrastructure;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts;
using OnlineBanking.Application.Features.CashTransactions.Commands;
using OnlineBanking.Application.Models;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Constants;
using OnlineBanking.Core.Domain.Enums;
using OnlineBanking.Core.Domain.Services.BankAccount;

namespace OnlineBanking.Application.Features.CashTransactions.CommandHandlers;

public class MakeWithdrawalCommandHandler(IUnitOfWork uow,
                                    IBankAccountService bankAccountService,
                                    IAppUserAccessor appUserAccessor) : IRequestHandler<MakeWithdrawalCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IBankAccountService _bankAccountService = bankAccountService;
    private readonly IAppUserAccessor _appUserAccessor = appUserAccessor;

    public async Task<ApiResult<Unit>> Handle(MakeWithdrawalCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();

        var loggedInAppUser = await _uow.AppUsers.GetAppUser(_appUserAccessor.GetUsername());

        var bankAccount = await _uow.BankAccounts.GetByIBANAsync(request.BaseCashTransaction.IBAN);

        if (bankAccount is null)
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, request.From));

            return result;
        }

        var bankAccountOwner = bankAccount.BankAccountOwners.FirstOrDefault(c => c.Customer.AppUserId == loggedInAppUser.Id)?.Customer;

        if (bankAccountOwner is null)
        {
            result.AddError(ErrorCode.CreateCashTransactionNotAuthorized,
            string.Format(CashTransactionErrorMessages.UnAuthorizedOperation, loggedInAppUser.UserName));

            return result;
        }

        var amountToWithdraw = request.BaseCashTransaction.Amount.Value;

        if (bankAccount.AllowedBalanceToUse < amountToWithdraw)
        {
            result.AddError(ErrorCode.InSufficintFunds, CashTransactionErrorMessages.InsufficientFunds);

            return result;
        }
        
        //Update account balance & Add transaction
        var updatedBalance = bankAccount.Balance - amountToWithdraw; 
        var sender = $"{bankAccountOwner.FirstName} {bankAccountOwner.LastName}";


        var cashTransaction = CreateCashTransaction(request, sender, updatedBalance);

        await _uow.CashTransactions.AddAsync(cashTransaction);

        bool createdTransaction = _bankAccountService.CreateCashTransaction(bankAccount, null, cashTransaction.Id, 
                                                                            amountToWithdraw, CashTransactionType.Withdrawal); 

        if (!createdTransaction)
        {
            result.AddError(ErrorCode.UnknownError, CashTransactionErrorMessages.UnknownError);

            return result;
        }

        _uow.BankAccounts.Update(bankAccount);

        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            cashTransaction.UpdateStatus(CashTransactionStatus.Completed);
            _uow.CashTransactions.Update(cashTransaction);

            await _uow.SaveAsync();
        }
        else 
        {
            result.AddError(ErrorCode.UnknownError, CashTransactionErrorMessages.UnknownError);
        }

        return result;
    }

    #region Private methods
    private static CashTransaction CreateCashTransaction(MakeWithdrawalCommand request, string sender, decimal updatedBalance)
    {
        var ct = request.BaseCashTransaction;

        return CashTransaction.Create(ct.Type, ct.InitiatedBy, request.From, GetInitiatorCode(ct.InitiatedBy), 
                                      ct.Amount.Value, ct.Amount.CurrencyId, 0,
                                      ct.Description, updatedBalance, 0,
                                      ct.PaymentType, DateTimeHelper.ConvertToDate(ct.TransactionDate), 
                                      sender, "Unknown");
    }


    private static string GetInitiatorCode(BankAssetType initiatedBy)
    {
        return initiatedBy == BankAssetType.ATM ? InitiatorCode.ATM :
                                    initiatedBy == BankAssetType.Branch ? InitiatorCode.Branch :
                                    initiatedBy == BankAssetType.POS ?
                                    InitiatorCode.POS : InitiatorCode.Unknown;
    }
    #endregion
}