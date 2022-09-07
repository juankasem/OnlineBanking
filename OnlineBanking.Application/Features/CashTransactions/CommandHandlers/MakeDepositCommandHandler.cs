using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

        var validator = new MakeDepositCommandValidator(_uow);

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
            var toBankAccount = await _uow.BankAccounts.GetByIBANAsync(request.To);

            if (toBankAccount is null)
            {
                result.AddError(ErrorCode.NotFound,
                string.Format(BankAccountErrorMessages.NotFound, request.To));

                return result;
            }
            
            if (!toBankAccount.BankAccountOwners.Any(b => b.Customer.AppUserId == loggedInAppUser.Id))
            {
                result.AddError(ErrorCode.CreateCashTransactionNotAuthorized,
                string.Format(CashTransactionErrorMessages.UnAuthorizedOperation, request.To));

                return result;
            }

            var amountToDeposit = request.BaseCashTransaction.Amount.Value;

            //Update account balance & Add transaction
            var updatedToBalance = toBankAccount.UpdateBalance(amountToDeposit, OperationType.Add);
            var accountTransaction = new AccountTransaction()
            {
                Account = toBankAccount,
                Transaction = CreateCashTransaction(request, updatedToBalance)
            };

            toBankAccount.AddTransaction(accountTransaction);

            await _uow.BankAccounts.UpdateAsync(toBankAccount);
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
    private CashTransaction CreateCashTransaction(MakeDepositCommand request, decimal updatedBalance)
    {
        var ct = request.BaseCashTransaction;

        return CashTransaction.Create(ct.ReferenceNo, ct.Type,
                                        ct.InitiatedBy, GetInitiatorCode(ct.InitiatedBy),
                                        request.To, ct.Amount.Value, ct.Amount.Currency.Id, 
                                        ct.Fees.Value, ct.Description, 0, updatedBalance,
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
