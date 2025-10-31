using MediatR;
using Microsoft.Extensions.Logging;
using OnlineBanking.Application.Contracts.Infrastructure;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts;
using OnlineBanking.Application.Features.CashTransactions.Commands;
using OnlineBanking.Application.Helpers;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CashTransaction;
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
        _logger.LogInformation("Start creating transfer from {from} to {to}", request.From, request.To);

        var result = new ApiResult<Unit>();
        var senderIBAN = request.From;

        var senderAccount = await _uow.BankAccounts.GetByIBANAsync(senderIBAN);

        if (!ValidateBankAccount(senderAccount, senderIBAN, result) || !await ValidateBanakAccountOwner(senderAccount, result))
        {
            return result;
        }

        var recipientIBAN = request.To;
        var recipientAccount = await _uow.BankAccounts.GetByIBANAsync(recipientIBAN);

        if (!ValidateBankAccount(recipientAccount, recipientIBAN, result))
        {
            return result;
        }
         
        var amountToTransfer = request.BaseCashTransaction.Amount.Value;
        var fees = amountToTransfer * 0.025M;
        var totalAmount = amountToTransfer + fees;

        if (!HasSufficientFunds(senderAccount, totalAmount, result))
        {
            return result;
        }

        //Prepare transfer dto
        var transferDto = PrepareTransferDto(senderAccount, recipientAccount, amountToTransfer, fees);

        var cashTransaction = CashTransactionHelper.CreateCashTransaction(request, transferDto);

        //Create transfer transaction 
        bool transactionCreated = _bankAccountService.CreateCashTransaction(senderAccount, 
                                                                            recipientAccount, 
                                                                            cashTransaction, 
                                                                            fees);
        if (!transactionCreated)
        {
            result.AddError(ErrorCode.UnknownError, CashTransactionErrorMessages.UnknownError);
            return result;
        }

        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            cashTransaction.UpdateStatus(CashTransactionStatus.Completed);
            _uow.CashTransactions.Update(cashTransaction);
                
            await _uow.SaveAsync();

            _logger.LogInformation("Transfer transaction of Id {transactionId} of amount {amount} is successfully created!", cashTransaction.Id, amountToTransfer);
        }
        else
        {
            result.AddError(ErrorCode.UnknownError, CashTransactionErrorMessages.UnknownError);
            _logger.LogError("Deposit transaction failed...Please try again.");
        }

        return result;
    }

    private static bool ValidateBankAccount(
        Core.Domain.Aggregates.BankAccountAggregate.BankAccount? bankAccount,
        string iban,
        ApiResult<Unit> result)
        {
        var success = true;

        if (bankAccount == null)
        {
            result.AddError(ErrorCode.BadRequest, string.Format(BankAccountErrorMessages.NotFound, "IBAN.", iban));
            success = false;
        }

        return success;
    }

    private async Task<bool> ValidateBanakAccountOwner(
        Core.Domain.Aggregates.BankAccountAggregate.BankAccount? bankAccount,
        ApiResult<Unit> result)
    {
        var success = true;
        var loggedInAppUser = await _uow.AppUsers.GetAppUser(_appUserAccessor.GetUsername());

        var senderAccountOwner = bankAccount.BankAccountOwners.FirstOrDefault(c => c.Customer.AppUserId == loggedInAppUser.Id)?.Customer;

        if (senderAccountOwner is null)
        {
            result.AddError(ErrorCode.CreateCashTransactionNotAuthorized,
            string.Format(CashTransactionErrorMessages.UnAuthorizedOperation, loggedInAppUser.UserName));
            success = false;
        }

        return success;
    }

    private static bool HasSufficientFunds(
        Core.Domain.Aggregates.BankAccountAggregate.BankAccount? senderAccount, 
        decimal totalAmount, 
        ApiResult<Unit> result)
    {
        if (senderAccount.AllowedBalanceToUse < totalAmount)
        {
            result.AddError(ErrorCode.InSufficintFunds, CashTransactionErrorMessages.InsufficientFunds);
            return false;
        }

        return true;
    }

    private static TransferDto PrepareTransferDto(Core.Domain.Aggregates.BankAccountAggregate.BankAccount? senderAccount, 
                                           Core.Domain.Aggregates.BankAccountAggregate.BankAccount? recipientAccount, 
                                           decimal amountToTransfer, 
                                           decimal fees)
    {
        var senderOwner = senderAccount.BankAccountOwners[0]?.Customer;
        var recipientOwner = recipientAccount.BankAccountOwners[0]?.Customer;
        var updatedSenderAccount = senderAccount.Balance - (amountToTransfer + fees);
        var updatedRecipientAccount = recipientAccount.Balance + amountToTransfer;

        return new TransferDto(
            $"{senderOwner.FirstName} {senderOwner.LastName}",
            $"{recipientOwner.FirstName} {recipientOwner.LastName}",
            updatedSenderAccount,
            updatedRecipientAccount,
            fees
        );
    }
}