
namespace OnlineBanking.Application.Features.FastTransactions.Create;

public class CreateFastTransactionCommandHandler(IUnitOfWork uow,
                                                IBankAccountService bankAccountService,
                                                ILogger<CreateFastTransactionCommandHandler> logger) :
                                                IRequestHandler<CreateFastTransactionCommand, ApiResult<Unit>>
{

    private readonly IUnitOfWork _uow = uow;
    private readonly IBankAccountService _bankAccountService = bankAccountService;
    private readonly ILogger<CreateFastTransactionCommandHandler> _logger = logger;

    public async Task<ApiResult<Unit>> Handle(CreateFastTransactionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Start creating fast transaction for {request.IBAN}");

        var result = new ApiResult<Unit>();

        var senderIBAN = request.IBAN;
        var bankAccount = await _uow.BankAccounts.GetByIBANAsync(request.IBAN);

        if (!ValidateBankAccount(bankAccount, senderIBAN, result))
            return result;

        var recipientIBAN = request.RecipientIBAN;
        var recipientBankAccount = await _uow.BankAccounts.GetByIBANAsync(request.RecipientIBAN);

        if (!ValidateBankAccount(recipientBankAccount, recipientIBAN, result))
            return result;

        var fastTransaction = FastTransaction.Create(bankAccount.Id, request.RecipientIBAN, 
                                                     request.RecipientName, request.Amount);

        //Add fast transaction to sender's account
        _bankAccountService.CreateFastTransaction(bankAccount, fastTransaction);

        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            _logger.LogInformation($"Fast transaction of Id {fastTransaction.Id} of amount "
                                   + $"{fastTransaction.Amount}{fastTransaction.BankAccount.Currency.Symbol} for bank account IBAN {fastTransaction.RecipientIBAN} with name "
                                   + $"{fastTransaction.RecipientName} is successfully created!");
        }
        else
        {
            result.AddError(ErrorCode.UnknownError, FastTransactionErrorMessages.Unknown);
            _logger.LogError($"Ceate fast transaction failed...Please try again.");
        }

        return result;
    }

    /// <summary>
    /// Validates that the bank account exists and is valid
    /// </summary>
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
}
