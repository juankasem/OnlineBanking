using MediatR;
using OnlineBanking.Application.Common.Helpers;
using OnlineBanking.Application.Contracts.Infrastructure;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts;
using OnlineBanking.Application.Features.CashTransactions.Commands;
using OnlineBanking.Application.Models;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Enums;
using OnlineBanking.Core.Domain.Exceptions;
using OnlineBanking.Core.Domain.Services.BankAccount;

namespace OnlineBanking.Application.Features.CashTransactions.CommandHandlers;

public class MakeFundsTransferCommandHandler : IRequestHandler<MakeFundsTransferCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IBankAccountService _bankAccountService;
    private readonly IAppUserAccessor _appUserAccessor;


    public MakeFundsTransferCommandHandler(IUnitOfWork uow, 
                                           IBankAccountService bankAccountService,  
                                           IAppUserAccessor appUserAccessor)
    {
        _uow = uow;
        _bankAccountService = bankAccountService;
        _appUserAccessor = appUserAccessor;
    }

    public async Task<ApiResult<Unit>> Handle(MakeFundsTransferCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();

        var userName = _appUserAccessor.GetUsername();
        var loggedInAppUser = await _uow.AppUsers.GetAppUser(userName);

        try
        {
            var senderAccount = await _uow.BankAccounts.GetByIBANAsync(request.From);

            if (senderAccount is null)
            {
                result.AddError(ErrorCode.NotFound,
                string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.From));

                return result;
            }

            var senderAccountOwner = senderAccount.BankAccountOwners.FirstOrDefault(c => c.Customer.AppUserId == loggedInAppUser.Id)?.Customer;

            if (senderAccountOwner is null)
            {
                result.AddError(ErrorCode.CreateCashTransactionNotAuthorized,
                string.Format(CashTransactionErrorMessages.UnAuthorizedOperation, request.From));

                return result;
            }

            var recipientAccount = await _uow.BankAccounts.GetByIBANAsync(request.To);

            if (recipientAccount is null)
            {
                result.AddError(ErrorCode.NotFound,
                string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.To));

                return result;
            }

            var amountToTransfer = request.BaseCashTransaction.Amount.Value;
            var fees = request.BaseCashTransaction.Fees.Value;

            if (senderAccount.AllowedBalanceToUse < amountToTransfer)
            {
                result.AddError(ErrorCode.InSufficintFunds, CashTransactionErrorMessages.InsufficientFunds);

                return result;
            }
             
            //Update sender's & Recipient's account
            var updatedFromBalance = senderAccount.Balance - (amountToTransfer + fees);
            var updatedToBalance = recipientAccount.Balance + amountToTransfer;

            var cashTransaction = CreateCashTransaction(request, updatedFromBalance, updatedToBalance);
            await _uow.CashTransactions.AddAsync(cashTransaction);
            
            //Create transfer transaction 
            bool createdTransaction = _bankAccountService.CreateCashTransaction(senderAccount, recipientAccount, cashTransaction.Id, amountToTransfer);

            if (!createdTransaction)
            {
                result.AddError(ErrorCode.UnknownError, CashTransactionErrorMessages.UnknownError);

                return result;
            }
        
            _uow.BankAccounts.Update(senderAccount);
            _uow.BankAccounts.Update(recipientAccount);

            if (await _uow.CompleteDbTransactionAsync() >= 1)
            {
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
    private static CashTransaction CreateCashTransaction(MakeFundsTransferCommand request, decimal updatedFromBalance, decimal updatedToBalance)
    {
        var ct = request.BaseCashTransaction;
        var transactionDate = DateTimeHelper.ConvertToDate(ct.TransactionDate);

        return CashTransaction.Create(ct.Type, ct.InitiatedBy,
                                      request.From, request.To, ct.Amount.Value, ct.Amount.CurrencyId,
                                      ct.Fees.Value, ct.Description, updatedFromBalance, updatedToBalance,
                                      ct.PaymentType, transactionDate, request.Sender, request.Recipient);
    }
    #endregion
}