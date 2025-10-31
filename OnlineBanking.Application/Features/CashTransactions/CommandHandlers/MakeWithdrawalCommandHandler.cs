using MediatR;
using Microsoft.Extensions.Logging;
using OnlineBanking.Application.Contracts.Infrastructure;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.CashTransactions.Commands;
using OnlineBanking.Application.Helpers;
using OnlineBanking.Application.Models;
using OnlineBanking.Core.Domain.Enums;
using OnlineBanking.Core.Domain.Services.BankAccount;

namespace OnlineBanking.Application.Features.CashTransactions.CommandHandlers;

    public class MakeWithdrawalCommandHandler(IUnitOfWork uow,
                                                IBankAccountService bankAccountService,
                                                IAppUserAccessor appUserAccessor,
                                                ILogger<MakeWithdrawalCommandHandler> logger) : 
                                                IRequestHandler<MakeWithdrawalCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IBankAccountService _bankAccountService = bankAccountService;
    private readonly IAppUserAccessor _appUserAccessor = appUserAccessor;
    private readonly ILogger<MakeWithdrawalCommandHandler> _logger = logger;

    public async Task<ApiResult<Unit>> Handle(MakeWithdrawalCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start creating withdrawal from {from}", request.From);

        var result = new ApiResult<Unit>();
        var bankAccountIBAN = request.BaseCashTransaction.IBAN;

        var bankAccount = await _uow.BankAccounts.GetByIBANAsync(bankAccountIBAN);

        if (!ValidateBankAccount(bankAccount, bankAccountIBAN, result) || !await ValidateBanakAccountOwner(bankAccount, result))
        {
            return result;
        }

        var amountToWithdraw = request.BaseCashTransaction.Amount.Value;

        if (!HasSufficientFunds(bankAccount, amountToWithdraw, result))
        {
            return result;
        }

        //Update account balance & Add transaction
        var updatedBalance = bankAccount.Balance - amountToWithdraw;
        var bankAccountOwner = bankAccount.BankAccountOwners[0]?.Customer;
        var ownerFullName = $"{bankAccountOwner?.FirstName} {bankAccountOwner?.LastName}";

        var cashTransaction = CashTransactionHelper.CreateCashTransaction(request, ownerFullName, updatedBalance);

        bool transactionCreated = _bankAccountService.CreateCashTransaction(bankAccount, null, cashTransaction);
                                            
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

            _logger.LogInformation("Withdrawal transaction of Id {cashTransactionId} of amount {amount} is created", cashTransaction.Id, amountToWithdraw);
        }
        else 
        {
            result.AddError(ErrorCode.UnknownError, CashTransactionErrorMessages.UnknownError);
            _logger.LogError($"Withdrawal transaction failed...Please try again.");
        }

        return result;
    }

    private static bool ValidateBankAccount(Core.Domain.Aggregates.BankAccountAggregate.BankAccount? bankAccount,
                                          string iban,
                                          ApiResult<Unit> result)
    {
        var success = true;
        if (bankAccount is null)
        {
            result.AddError(ErrorCode.BadRequest, $"Sender account with IBAN {iban} not found.");
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

    private static bool HasSufficientFunds(Core.Domain.Aggregates.BankAccountAggregate.BankAccount? bankAccount, decimal totalAmount, ApiResult<Unit> result)
    {
        if (bankAccount.AllowedBalanceToUse < totalAmount)
        {
            result.AddError(ErrorCode.InSufficintFunds, CashTransactionErrorMessages.InsufficientFunds);
            return false;
        }

        return true;
    }
}