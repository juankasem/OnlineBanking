
namespace OnlineBanking.Application.Features.FastTransactions.CommandHandlers;

public class CreateFastTransactionCommandHandler(IUnitOfWork uow,
                                                IBankAccountService bankAccountService,
                                                IAppUserAccessor appUserAccessor,
                                                ILogger<CreateFastTransactionCommandHandler> logger) :
                                                IRequestHandler<CreateFastTransactionCommand, ApiResult<Unit>>
{

    private readonly IUnitOfWork _uow = uow;
    private readonly IBankAccountService _bankAccountService = bankAccountService;
    private readonly IAppUserAccessor _appUserAccessor = appUserAccessor;
    private readonly ILogger<CreateFastTransactionCommandHandler> _logger = logger;

    public async Task<ApiResult<Unit>> Handle(CreateFastTransactionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Start creating fast transaction for {request.IBAN}");

        var result = new ApiResult<Unit>();

        var userName = _appUserAccessor.GetUsername();
        var loggedInAppUser = await _uow.AppUsers.GetAppUser(userName);

        var bankAccount = await _uow.BankAccounts.GetByIBANAsync(request.IBAN);

        if (bankAccount is null)
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.IBAN));

            return result;
        }

        var recipientBankAccount = await _uow.BankAccounts.GetByIBANAsync(request.RecipientIBAN);

        if (recipientBankAccount is null)
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.RecipientIBAN));

            return result;
        }

        var fastTransaction = FastTransaction.Create(bankAccount.Id, request.RecipientIBAN, request.RecipientName, request.Amount);

        //Add fast transaction to sender's account
        var fastTransactionCreated = _bankAccountService.CreateFastTransaction(bankAccount, fastTransaction);

        if (!fastTransactionCreated)
        {
            result.AddError(ErrorCode.UnknownError, FastTransactionErrorMessages.UnknownError);

            return result;
        }

        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            _logger.LogInformation($"Fast transaction of Id {fastTransaction.Id} of amount "
                                   + $"{fastTransaction.Amount}{fastTransaction.BankAccount.Currency.Symbol} for bank account IBAN {fastTransaction.RecipientIBAN} with name "
                                   + $"{fastTransaction.RecipientName} is successfully created!");
        }
        else
        {
            result.AddError(ErrorCode.UnknownError, FastTransactionErrorMessages.UnknownError);
            _logger.LogError($"Ceate fast transaction failed...Please try again.");
        }

        return result;
    }
}
