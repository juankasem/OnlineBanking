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

public class MakeDepositCommandHandler(IUnitOfWork uow,
                                 IBankAccountService bankAccountService,
                                 IAppUserAccessor appUserAccessor) : IRequestHandler<MakeDepositCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IBankAccountService _bankAccountService = bankAccountService;
    private readonly IAppUserAccessor _appUserAccessor = appUserAccessor;

    public async Task<ApiResult<Unit>> Handle(MakeDepositCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();

        var userName = _appUserAccessor.GetUsername();
        var loggedInAppUser = await _uow.AppUsers.GetAppUser(userName);

        var bankAccount = await _uow.BankAccounts.GetByIBANAsync(request.BaseCashTransaction.IBAN);

        if (bankAccount is null)
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, request.To));

            return result;
        }

        var bankAccountOwner = bankAccount.BankAccountOwners.FirstOrDefault(c => c.Customer.AppUserId == loggedInAppUser.Id)?.Customer;

        if (bankAccountOwner is null)
        {
            result.AddError(ErrorCode.CreateCashTransactionNotAuthorized,
            string.Format(CashTransactionErrorMessages.UnAuthorizedOperation, request.BaseCashTransaction.IBAN));

            return result;
        }

        var amountToDeposit = request.BaseCashTransaction.Amount.Value;

        //Update account balance & Add transaction
        var updatedBalance = bankAccount.Balance + amountToDeposit;
        var recipient = bankAccountOwner.FirstName + " " + bankAccountOwner.LastName;

        var cashTransaction = CreateCashTransaction(request, recipient, updatedBalance);

        await _uow.CashTransactions.AddAsync(cashTransaction);

        bool createdTransaction = _bankAccountService.CreateCashTransaction(null, bankAccount, cashTransaction.Id, amountToDeposit, CashTransactionType.Deposit);

        if (!createdTransaction)
        {
            result.AddError(ErrorCode.UnknownError, CashTransactionErrorMessages.UnknownError);

            return result;
        }

        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            cashTransaction.UpdateStatus(CashTransactionStatus.Completed);
            _uow.BankAccounts.Update(bankAccount);

            await _uow.SaveAsync();
        }
        else
        {
            result.AddError(ErrorCode.UnknownError, CashTransactionErrorMessages.UnknownError);
        }

        return result;
    }

    #region Private methods
    private static CashTransaction CreateCashTransaction(MakeDepositCommand request, string recipient, decimal updatedBalance)
    {
        var ct = request.BaseCashTransaction;
        var transactionDate = DateTimeHelper.ConvertToDate(ct.TransactionDate);

        return CashTransaction.Create(ct.Type, ct.InitiatedBy, 
                                        GetInitiatorCode(ct.InitiatedBy),
                                        request.To, ct.Amount.Value, ct.Amount.CurrencyId, 
                                        0, ct.Description, 0, updatedBalance,
                                        ct.PaymentType, transactionDate, "Unknown", recipient);
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
