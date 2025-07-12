using MediatR;
using Microsoft.Extensions.Logging;
using OnlineBanking.Application.Contracts.Infrastructure;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.CashTransactions.Commands;
using OnlineBanking.Application.Helpers;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CashTransaction;
using OnlineBanking.Core.Domain.Entities;
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
        var loggedInAppUser = await _uow.AppUsers.GetAppUser(_appUserAccessor.GetUsername());

        var senderAccount = await _uow.BankAccounts.GetByIBANAsync(request.From);

        if (!ValidateSenderAccount(senderAccount, loggedInAppUser, result))
        {
            return result;
        }

        var recipientAccount = await _uow.BankAccounts.GetByIBANAsync(request.To);

        if (!ValidateRecipientAccount(recipientAccount, result))
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
        await _uow.CashTransactions.AddAsync(cashTransaction);

        //Create transfer transaction 
        bool transactionCreated = _bankAccountService.CreateCashTransaction(senderAccount, 
                                                                            recipientAccount, 
                                                                            cashTransaction.Id,
                                                                            amountToTransfer, fees, 
                                                                            CashTransactionType.Transfer);
        if (!transactionCreated)
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

            _logger.LogInformation("Transfer transaction of Id {transactionId} of amount {amount} is successfully created!", cashTransaction.Id, amountToTransfer);
        }
        else
        {
            result.AddError(ErrorCode.UnknownError, CashTransactionErrorMessages.UnknownError);
            _logger.LogError("Deposit transaction failed...Please try again.");
        }

        return result;
    }

    private static bool ValidateSenderAccount(Core.Domain.Aggregates.BankAccountAggregate.BankAccount? senderAccount, 
                                            AppUser loggedInAppUser, 
                                            ApiResult<Unit> result)
        {
        if (senderAccount == null)
        {
            result.AddError(ErrorCode.BadRequest, "Sender account with IBAN not found.");

            return false;
        }

        var senderAccountOwner = senderAccount.BankAccountOwners.FirstOrDefault(c => c.Customer.AppUserId == loggedInAppUser.Id)?.Customer;

        if (senderAccountOwner is null)
        {
            result.AddError(ErrorCode.CreateCashTransactionNotAuthorized,
            string.Format(CashTransactionErrorMessages.UnAuthorizedOperation, loggedInAppUser.UserName));
            return false;
        }

        return true;
    }

    private static bool ValidateRecipientAccount(Core.Domain.Aggregates.BankAccountAggregate.BankAccount? recipientAccount, ApiResult<Unit> result)
    {
        if (recipientAccount == null)
        {
            result.AddError(ErrorCode.BadRequest, "Recipient account not found.");
            return false;
        }
        return true;
    }

    private static bool HasSufficientFunds(Core.Domain.Aggregates.BankAccountAggregate.BankAccount? senderAccount, decimal totalAmount, ApiResult<Unit> result)
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
        var senderOwner = senderAccount.BankAccountOwners[0].Customer;
        var recipientOwner = recipientAccount.BankAccountOwners[0].Customer;
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