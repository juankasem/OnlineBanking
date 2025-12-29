
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
        _logger.LogInformation("Start creating fast transaction for bank account IBAN {iban}", request.IBAN);
        var result = new ApiResult<Unit>();

        var senderIBAN = request.IBAN;
        var bankAccount = await _uow.BankAccounts.GetByIBANAsync(request.IBAN);

        if (!BankAccountHelper.ValidateBankAccount(bankAccount, senderIBAN, result))
            return result;

        var recipientIBAN = request.RecipientIBAN;
        var recipientBankAccount = await _uow.BankAccounts.GetByIBANAsync(request.RecipientIBAN);

        if (!BankAccountHelper.ValidateBankAccount(recipientBankAccount, recipientIBAN, result))
            return result;

        var fastTransaction = FastTransaction.Create(bankAccount.Id, 
                                                     request.RecipientIBAN, 
                                                     request.RecipientName, 
                                                     request.Amount);

        //Add fast transaction to sender's account
        _bankAccountService.CreateFastTransaction(bankAccount, fastTransaction);

        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            _logger.LogInformation("Fast transaction of Id {fastTransactionId} of amount: "
                         + "{amount}{currency} for bank account IBAN {recipientIBAN} with name: "
                         + "{recipientName} is successfully created!",
                         fastTransaction.Id,
                         fastTransaction.Amount,
                         fastTransaction.BankAccount.Currency.Symbol,
                         fastTransaction.RecipientIBAN,
                         fastTransaction.RecipientName);
        }
        else
        {
            result.AddError(ErrorCode.UnknownError, FastTransactionErrorMessages.Unknown);
            _logger.LogError($"Creating fast transaction failed...Please try again.");
        }

        return result;
    }
}
