
namespace OnlineBanking.Application.Features.CashTransactions.Create.Deposit;

public class MakeDepositCommandHandler(IUnitOfWork uow,
                                        IBankAccountService bankAccountService,
                                        IAppUserAccessor appUserAccessor,
                                        ILogger<MakeDepositCommandHandler> logger) :
                                        IRequestHandler<MakeDepositCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IBankAccountService _bankAccountService = bankAccountService;
    private readonly IAppUserAccessor _appUserAccessor = appUserAccessor;
    private readonly ILogger<MakeDepositCommandHandler> _logger = logger;

    public async Task<ApiResult<Unit>> Handle(MakeDepositCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Start creating deposit to {request.To}");

        var result = new ApiResult<Unit>();

        var bankAccount = await _uow.BankAccounts.GetByIBANAsync(request.BaseCashTransaction.IBAN);

        if (bankAccount is null)
        {
            result.AddError(ErrorCode.BadRequest,
            string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.To));
            return result;
        }

        var recipient = await GetBankAccountOwner(bankAccount);
        var amountToDeposit = request.BaseCashTransaction.Amount.Value;

        //Update account balance & Add transaction
        var updatedBalance = bankAccount.Balance + amountToDeposit;

        var cashTransaction = CashTransactionHelper.CreateCashTransaction(request, recipient, updatedBalance);

        _bankAccountService.CreateCashTransaction(null, bankAccount, cashTransaction);

        // mark business result as completed on the object before saving so EF persists it in same transaction
        cashTransaction.UpdateStatus(CashTransactionStatus.Completed);

        // Mark aggregate as modified so it will be saved
        _uow.BankAccounts.Update(bankAccount);

        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            _logger.LogInformation($"Deposit transaction of Id {cashTransaction.Id} of amount {amountToDeposit} is created");
        }
        else
        {
            result.AddError(ErrorCode.UnknownError, CashTransactionErrorMessages.UnknownError);
            _logger.LogError($"Deposit transactyion failed");
        }

        return result;
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
}
