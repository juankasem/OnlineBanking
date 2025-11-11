
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

        if (!ValidateBankAccount(bankAccount, bankAccountIBAN, result))
        {
            return result;
        }

        var bankAccountOwnerName = await GetBankAccountOwner(bankAccount);
        var amountToWithdraw = request.BaseCashTransaction.Amount.Value;

        if (!HasSufficientFunds(bankAccount, amountToWithdraw, result))
        {
            return result;
        }

        //Update account balance & Add transaction
        var updatedBalance = bankAccount.Balance - amountToWithdraw;

        var cashTransaction = CashTransactionHelper.CreateCashTransaction(request, bankAccountOwnerName, updatedBalance);

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

            _logger.LogInformation("Withdrawal transaction of Id {cashTransactionId} of amount {amount} is created",
                                    cashTransaction.Id,
                                    amountToWithdraw);
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

    private async Task<string> GetBankAccountOwner(
        Core.Domain.Aggregates.BankAccountAggregate.BankAccount? bankAccount)
    {
        var loggedInAppUser = await _uow.AppUsers.GetAppUser(_appUserAccessor.GetUsername());

        if (loggedInAppUser is null)
        {
            return string.Empty;
        }

        var bankAccountOwner = bankAccount.BankAccountOwners.FirstOrDefault(c => c.Customer.AppUserId == loggedInAppUser.Id)?.Customer;

        return bankAccountOwner is not null ? bankAccountOwner.FirstName + " " + bankAccountOwner.LastName :
                string.Empty;
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