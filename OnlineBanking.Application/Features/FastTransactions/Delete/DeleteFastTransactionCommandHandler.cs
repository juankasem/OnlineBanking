
namespace OnlineBanking.Application.Features.FastTransactions.Delete;

public class DeleteFastTransactionCommandHandler(IUnitOfWork uow,
                                                 IBankAccountService bankAccountService,
                                                 ILogger<DeleteFastTransactionCommandHandler> logger) : 
                                                 IRequestHandler<DeleteFastTransactionCommand, ApiResult<Unit>>
{

    private readonly IUnitOfWork _uow = uow;
    private readonly IBankAccountService _bankAccountService = bankAccountService;
    private readonly ILogger<DeleteFastTransactionCommandHandler> _logger = logger;

    public async Task<ApiResult<Unit>> Handle(DeleteFastTransactionCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();

        var bankAccount = await _uow.BankAccounts.GetByIBANAsync(request.IBAN);
        if (bankAccount is null)
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.IBAN));
            return result;
        }

        //Delete fast transaction from account
        _bankAccountService.DeleteFastTransation(request.Id, bankAccount);

        //Update sender's account
        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            _logger.LogInformation("Fast transaction of Id {fastTransactionId} is successfully deleted", request.Id);
        }
        else
        {
            result.AddError(ErrorCode.UnknownError, FastTransactionErrorMessages.Unknown);
            _logger.LogError($"Ceate fast transaction failed...Please try again.");
        }  

        return result;
    }
}