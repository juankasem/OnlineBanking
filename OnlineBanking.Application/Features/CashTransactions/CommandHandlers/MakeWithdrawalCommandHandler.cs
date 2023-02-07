using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OnlineBanking.Application.Contracts.Infrastructure;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts;
using OnlineBanking.Application.Features.CashTransactions.Commands;
using OnlineBanking.Application.Features.CashTransactions.Validators;
using OnlineBanking.Application.Models;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Constants;
using OnlineBanking.Core.Domain.Enums;
using OnlineBanking.Core.Domain.Exceptions;

namespace OnlineBanking.Application.Features.CashTransactions.CommandHandlers;

public class MakeWithdrawalCommandHandler : IRequestHandler<MakeWithdrawalCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IAppUserAccessor _appUserAccessor;
    public MakeWithdrawalCommandHandler(IUnitOfWork uow, IAppUserAccessor appUserAccessor)
    {
        _uow = uow;
        _appUserAccessor = appUserAccessor;
    }

    public async Task<ApiResult<Unit>> Handle(MakeWithdrawalCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();

        var validator = new MakeWithdrawalCommandValidator(_uow);

        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            validationResult.Errors.ForEach(er => result.AddError(ErrorCode.ValidationError, er.ErrorMessage));

            return result;
        }
        
        var userName = _appUserAccessor.GetUsername();
        var loggedInAppUser = await _uow.AppUsers.GetAppUser(userName);

        using var dbContextTransaction = await _uow.CreateDbTransactionAsync();

        try
        {
            var fromBankAccount = await _uow.BankAccounts.GetByIBANAsync(request.From);

            if (fromBankAccount is null)
            {
                result.AddError(ErrorCode.NotFound,
                string.Format(BankAccountErrorMessages.NotFound, request.From));

                return result;
            }

            if (!fromBankAccount.BankAccountOwners.Any(b => b.Customer.AppUserId == loggedInAppUser.Id))
            {
                result.AddError(ErrorCode.CreateCashTransactionNotAuthorized,
                string.Format(CashTransactionErrorMessages.UnAuthorizedOperation, request.From));

                return result;
            }

            var amountToWithdraw = request.BaseCashTransaction.Amount.Value;

            if (fromBankAccount.AllowedBalanceToUse < amountToWithdraw)
            {
                result.AddError(ErrorCode.InSufficintFunds, CashTransactionErrorMessages.InsufficientFunds);
                
                return result;
            }

           //Update account balance & Add transaction
            var updatedFromBalance = fromBankAccount.UpdateBalance(amountToWithdraw, OperationType.Subtract);
            var accountTransaction = new AccountTransaction()
            {
                Account = fromBankAccount,
                Transaction = CreateCashTransaction(request, updatedFromBalance)
            };
            
            fromBankAccount.AddTransaction(accountTransaction);

            await _uow.BankAccounts.UpdateAsync(fromBankAccount);
            await dbContextTransaction.CommitAsync();

            return result;
        }
        catch (CashTransactionNotValidException e)
        {
            await dbContextTransaction.RollbackAsync();
            e.ValidationErrors.ForEach(er => result.AddError(ErrorCode.ValidationError, er));
        }
        catch (Exception e)
        {
            await dbContextTransaction.RollbackAsync();
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    #region Private methods
    private CashTransaction CreateCashTransaction(MakeWithdrawalCommand request, decimal updatedBalance)
    {
        var ct = request.BaseCashTransaction;

        return CashTransaction.Create(ct.ReferenceNo, ct.Type,
                                    ct.InitiatedBy, request.From, GetInitiatorCode(ct.InitiatedBy), ct.Amount.Value,
                                    ct.Amount.Currency.Id, ct.Fees.Value, ct.Description, updatedBalance, 0,
                                    ct.PaymentType, ct.TransactionDate, ct.Status);
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