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
using OnlineBanking.Core.Domain.Exceptions;

namespace OnlineBanking.Application.Features.CashTransactions.CommandHandlers;

public class MakeFundsTransferCommandHandler : IRequestHandler<MakeFundsTransferCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IAppUserAccessor _appUserAccessor;

    public MakeFundsTransferCommandHandler(IUnitOfWork uow, IAppUserAccessor appUserAccessor)
    {
        _uow = uow;
        _appUserAccessor = appUserAccessor;
    }

    public async Task<ApiResult<Unit>> Handle(MakeFundsTransferCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();
        var validator = new MakeFundsTransferCommandValidator(_uow);

        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            validationResult.Errors.ForEach(er => result.AddError(ErrorCode.ValidationError, er.ErrorMessage));

            return result;
        }

        var userName = _appUserAccessor.GetUsername();
        var loggedInAppUser = await _uow.AppUsers.GetAppUser(userName);

        //Start database transaction
        using var dbContextTransaction = await _uow.CreateDbTransactionAsync();

        try
        {
            //Perform validations
            var fromBankAccount = await _uow.BankAccounts.GetByIBANAsync(request.From);

            if (fromBankAccount is null)
            {
                result.AddError(ErrorCode.NotFound,
                string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.From));

                return result;
            }

            if (!fromBankAccount.BankAccountOwners.Any(b => b.Customer.AppUserId == loggedInAppUser.Id))
            {
                result.AddError(ErrorCode.CreateCashTransactionNotAuthorized,
                string.Format(CashTransactionErrorMessages.UnAuthorizedOperation, request.From));

                return result;
            }

            var toBankAccount = await _uow.BankAccounts.GetByIBANAsync(request.From);

            if (toBankAccount is null)
            {
                result.AddError(ErrorCode.NotFound,
                string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.To));

                return result;
            }

            var amountToTransfer = request.BaseCashTransaction.Amount.Value;

            if (fromBankAccount.AllowedBalanceToUse < amountToTransfer)
            {
                result.AddError(ErrorCode.InSufficintFunds, CashTransactionErrorMessages.InsufficientFunds);
            
                return result;
            }
            
            //Update sender's account
            var updatedFromBalance = fromBankAccount.UpdateBalance(amountToTransfer, OperationType.Subtract);
            await _uow.SaveAsync();

            //Update recipient's account
            var updatedToBalance = toBankAccount.UpdateBalance(amountToTransfer, OperationType.Add);
            await _uow.SaveAsync();

            var cashTransaction = CreateCashTransaction(request, updatedFromBalance, updatedToBalance);

            await _uow.CashTransactions.AddAsync(cashTransaction);
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

    #region Private helper methods
    private CashTransaction CreateCashTransaction(MakeFundsTransferCommand request,
                                                decimal updatedFromBalance,
                                                decimal updatedToBalance)
    {
        var ct = request.BaseCashTransaction;

        return CashTransaction.Create(ct.ReferenceNo, ct.Type, ct.InitiatedBy,
                                    request.From, request.To, ct.Amount.Value, ct.Amount.Currency.Id, 
                                    ct.Fees.Value, ct.Description, updatedFromBalance, updatedToBalance,
                                    ct.PaymentType, ct.TransactionDate, ct.Status);
    }
    #endregion
}