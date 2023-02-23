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

        var userName = _appUserAccessor.GetUsername();
        var loggedInAppUser = await _uow.AppUsers.GetAppUser(userName);

        try
        {
            var fromAccount = await _uow.BankAccounts.GetByIBANAsync(request.From);

            if (fromAccount is null)
            {
                result.AddError(ErrorCode.NotFound,
                string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.From));

                return result;
            }

            if (!fromAccount.BankAccountOwners.Any(b => b.Customer.AppUserId == loggedInAppUser.Id))
            {
                result.AddError(ErrorCode.CreateCashTransactionNotAuthorized,
                string.Format(CashTransactionErrorMessages.UnAuthorizedOperation, request.From));

                return result;
            }

            var toAccount = await _uow.BankAccounts.GetByIBANAsync(request.From);

            if (toAccount is null)
            {
                result.AddError(ErrorCode.NotFound,
                string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.To));

                return result;
            }

            var amountToTransfer = request.BaseCashTransaction.Amount.Value;

            if (fromAccount.AllowedBalanceToUse < amountToTransfer)
            {
                result.AddError(ErrorCode.InSufficintFunds, CashTransactionErrorMessages.InsufficientFunds);

                return result;
            }

            //Update sender's & Recipient's account
            var updatedFromBalance = fromAccount.UpdateBalance(amountToTransfer, OperationType.Subtract);
            var updatedToBalance = toAccount.UpdateBalance(amountToTransfer, OperationType.Add);

            var transaction = CreateCashTransaction(request, updatedFromBalance, updatedToBalance);

            //Add transaction to sender's account
            fromAccount.AddTransaction(CreateAccountTransaction(fromAccount, transaction));

            //Update sender's account
            _uow.BankAccounts.Update(fromAccount);

            //Add transaction to recipient's account
            toAccount.AddTransaction(CreateAccountTransaction(toAccount, transaction));

            //Update recipient's account
            _uow.BankAccounts.Update(toAccount);

            //Update transaction status to 'complete'
            transaction.UpdateStatus(CashTransactionStatus.Completed);
            _uow.CashTransactions.Update(transaction);

            if (await _uow.CompleteDbTransactionAsync() >= 1)
            {
                var cashTransaction = await _uow.CashTransactions.GetByIdAsync(transaction.Id);
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

    #region Private helper methods
    private CashTransaction CreateCashTransaction(MakeFundsTransferCommand request,
                                                decimal updatedFromBalance,
                                                decimal updatedToBalance)
    {
        var ct = request.BaseCashTransaction;

        return CashTransaction.Create(ct.ReferenceNo, ct.Type, ct.InitiatedBy,
                                    request.From, request.To, ct.Amount.Value, ct.Amount.Currency.Id,
                                    ct.Fees.Value, ct.Description, updatedFromBalance, updatedToBalance,
                                    ct.PaymentType, ct.TransactionDate);
    }

    private AccountTransaction CreateAccountTransaction(OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate.BankAccount account,
                                                        CashTransaction transaction) =>
        new()
        {
            Account = account,
            Transaction = transaction
        };
    #endregion
}