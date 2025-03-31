using MediatR;
using Microsoft.Extensions.Logging;
using OnlineBanking.Application.Contracts.Infrastructure;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts;
using OnlineBanking.Application.Features.CashTransactions.Commands;
using OnlineBanking.Application.Helpers;
using OnlineBanking.Application.Models;
using OnlineBanking.Core.Domain.Enums;
using OnlineBanking.Core.Domain.Services.BankAccount;

namespace OnlineBanking.Application.Features.CashTransactions.CommandHandlers;

public class MakeFundsTransferCommandHandler(IUnitOfWork uow,
                                             IBankAccountService bankAccountService,
                                             IAppUserAccessor appUserAccessor,
                                             ILogger<MakeFundsTransferCommandHandler> logger) : 
                                             IRequestHandler<MakeFundsTransferCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IBankAccountService _bankAccountService = bankAccountService;
    private readonly IAppUserAccessor _appUserAccessor = appUserAccessor;
    private readonly ILogger<MakeFundsTransferCommandHandler> _logger = logger;

    public async Task<ApiResult<Unit>> Handle(MakeFundsTransferCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Start creating transfer from {request.From} to {request.To}");

        var result = new ApiResult<Unit>();
        var loggedInAppUser = await _uow.AppUsers.GetAppUser(_appUserAccessor.GetUsername());
      
        var senderAccount = await _uow.BankAccounts.GetByIBANAsync(request.From);

        if (senderAccount is null)
        {
            result.AddError(ErrorCode.BadRequest,
            string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.From));

            return result;
        }

        var senderAccountOwner = senderAccount.BankAccountOwners.FirstOrDefault(c => c.Customer.AppUserId == loggedInAppUser.Id)?.Customer;

        if (senderAccountOwner is null)
        {
            result.AddError(ErrorCode.CreateCashTransactionNotAuthorized,
            string.Format(CashTransactionErrorMessages.UnAuthorizedOperation, loggedInAppUser.UserName));

            return result;
        }

        var recipientAccount = await _uow.BankAccounts.GetByIBANAsync(request.To);

        if (recipientAccount is null)
        {
            result.AddError(ErrorCode.BadRequest,
            string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.To));

            return result;
        }

        var recipientAccountOwner = recipientAccount.BankAccountOwners.Select(c => c.Customer).FirstOrDefault();

        var amountToTransfer = request.BaseCashTransaction.Amount.Value;
        var fees = amountToTransfer * 0.025M;
        var amountWithFees = amountToTransfer + fees;

        if (senderAccount.AllowedBalanceToUse < amountWithFees)
        {
            result.AddError(ErrorCode.InSufficintFunds, CashTransactionErrorMessages.InsufficientFunds);

            return result;
        }

        //Update sender's & Recipient's account
        var updatedFromBalance = senderAccount.Balance - amountWithFees;
        var updatedToBalance = recipientAccount.Balance + amountToTransfer;
        var sender = $"{senderAccountOwner.FirstName} {senderAccountOwner.LastName}";
        var recipient = $"{recipientAccountOwner.FirstName} {recipientAccountOwner.LastName}";

        var cashTransaction = CashTransactionHelper.CreateCashTransaction(request, sender, recipient, updatedFromBalance, updatedToBalance, fees);
        await _uow.CashTransactions.AddAsync(cashTransaction);

        //Create transfer transaction 
        bool createdTransaction = _bankAccountService.CreateCashTransaction(senderAccount, recipientAccount, cashTransaction.Id,
                                                                            amountToTransfer, fees, CashTransactionType.Transfer);
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

            _logger.LogInformation($"Transfer transaction of Id {cashTransaction.Id} of amount {amountToTransfer} is successfully created!");
        }
        else
        {
            result.AddError(ErrorCode.UnknownError, CashTransactionErrorMessages.UnknownError);
            _logger.LogError($"Deposit transaction failed...Please try again.");
        }

        return result;
    }
}