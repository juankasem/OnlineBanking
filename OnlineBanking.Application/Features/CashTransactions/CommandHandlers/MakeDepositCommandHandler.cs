using MediatR;
using OnlineBanking.Application.Contracts.Infrastructure;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts;
using OnlineBanking.Application.Features.CashTransactions.Commands;
using OnlineBanking.Application.Models;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Constants;
using OnlineBanking.Core.Domain.Enums;
using OnlineBanking.Core.Domain.Exceptions;

namespace OnlineBanking.Application.Features.CashTransactions.CommandHandlers;

public class MakeDepositCommandHandler : IRequestHandler<MakeDepositCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IAppUserAccessor _appUserAccessor;
    public MakeDepositCommandHandler(IUnitOfWork uow, IAppUserAccessor appUserAccessor)
    {
        _uow = uow;
        _appUserAccessor = appUserAccessor;
    }

    public async Task<ApiResult<Unit>> Handle(MakeDepositCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();

        var userName = _appUserAccessor.GetUsername();
        var loggedInAppUser = await _uow.AppUsers.GetAppUser(userName);

        try
        {
            var bankAccount = await _uow.BankAccounts.GetByIBANAsync(request.BaseCashTransaction.IBAN);

            if (bankAccount is null)
            {
                result.AddError(ErrorCode.NotFound,
                string.Format(BankAccountErrorMessages.NotFound, request.To));

                return result;
            }
            
            if (!bankAccount.BankAccountOwners.Any(b => b.Customer.AppUserId == loggedInAppUser.Id))
            {
                result.AddError(ErrorCode.CreateCashTransactionNotAuthorized,
                string.Format(CashTransactionErrorMessages.UnAuthorizedOperation, request.BaseCashTransaction.IBAN));

                return result;
            }

            var amountToDeposit = request.BaseCashTransaction.Amount.Value;

            //Update account balance & Add transaction
            var updatedBalance = bankAccount.UpdateBalance(amountToDeposit, OperationType.Add);
            var accountTransaction = new AccountTransaction()
            {
                Account = bankAccount,
                Transaction = CreateCashTransaction(request, updatedBalance)
            };

            bankAccount.AddTransaction(accountTransaction);

            _uow.BankAccounts.Update(bankAccount);
        
            if (await _uow.CompleteDbTransactionAsync() >= 1)
            {
                var cashTransaction = await _uow.CashTransactions.GetByIdAsync(accountTransaction.TransactionId);
                cashTransaction.UpdateStatus(CashTransactionStatus.Completed);
                _uow.CashTransactions.Update(cashTransaction);
            
                await _uow.SaveAsync();
            }

            return result;
        }
        catch (CashTransactionNotValidException e)
        {
            e.ValidationErrors.ForEach(er => result.AddError(ErrorCode.ValidationError, er));
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    #region Private methods
    private CashTransaction CreateCashTransaction(MakeDepositCommand request, decimal updatedBalance)
    {
        var ct = request.BaseCashTransaction;

        return CashTransaction.Create(ct.ReferenceNo, ct.Type,
                                        ct.InitiatedBy, GetInitiatorCode(ct.InitiatedBy),
                                        request.To, ct.Amount.Value, ct.Amount.Currency.Id, 
                                        ct.Fees.Value, ct.Description, 0, updatedBalance,
                                        ct.PaymentType, ct.TransactionDate);
    }

    private string GetInitiatorCode(BankAssetType initiatedBy)
    {
        return initiatedBy == BankAssetType.ATM ? InitiatorCode.ATM :
                                    initiatedBy == BankAssetType.Branch ? InitiatorCode.Branch :
                                    initiatedBy == BankAssetType.POS ?
                                    InitiatorCode.POS : InitiatorCode.Unknown;
    }
    #endregion
}
