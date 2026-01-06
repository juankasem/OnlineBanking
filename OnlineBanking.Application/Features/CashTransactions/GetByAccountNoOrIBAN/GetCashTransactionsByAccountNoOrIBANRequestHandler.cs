using OnlineBanking.Application.Extensions;
using OnlineBanking.Application.Models.CashTransaction.Responses;

namespace OnlineBanking.Application.Features.CashTransactions.GetByAccountNoOrIBAN;

/// <summary>
/// Handles requests to retrieve cash transactions for a specific bank account.
/// Retrieves paginated transaction history and maps to response models.
/// </summary>
public class GetCashTransactionsByAccountNoOrIBANRequestHandler : IRequestHandler<GetCashTransactionsByAccountNoOrIBANRequest, ApiResult<PagedList<CashTransactionResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICashTransactionsMapper _cashTransactionsMapper;
    private readonly ILogger<GetCashTransactionsByAccountNoOrIBANRequestHandler> _logger;

    public GetCashTransactionsByAccountNoOrIBANRequestHandler(IUnitOfWork uow, 
        ICashTransactionsMapper cashTransactionsMapper,
        ILogger<GetCashTransactionsByAccountNoOrIBANRequestHandler> logger)
    {
        _uow = uow;
        _cashTransactionsMapper = cashTransactionsMapper;
        _logger = logger;
    }

    public async Task<ApiResult<PagedList<CashTransactionResponse>>> Handle(GetCashTransactionsByAccountNoOrIBANRequest request, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = new ApiResult<PagedList<CashTransactionResponse>>();
        var accountIdentifier = request.AccountNoOrIBAN;

        _logger.LogInformation("Retrieving transactions for account: {accountIdentifier}", accountIdentifier);

        if (!await _uow.BankAccounts.ExistsAsync(accountIdentifier))
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, "Account No/IBAN", accountIdentifier));
            return result;
        }

        var cashTransactionParams = request.CashTransactionParams;
        var (cashTransactions, totalCount) = await _uow.CashTransactions.GetByAccountNoOrIBANAsync(accountIdentifier, 
            cashTransactionParams);

        if (!cashTransactions.Any())
        {
            _logger.LogInformation("No transactions found for account: {ccountIdentifier}", accountIdentifier);
            return result;
        }

        var mappedCashTransactions = cashTransactions
            .Select(ct => _cashTransactionsMapper.MapToResponseModel(ct, accountIdentifier))
            .ToList()
            .AsReadOnly();

        result.Payload = mappedCashTransactions.ToPagedList(totalCount, 
            cashTransactionParams.PageNumber, 
            cashTransactionParams.PageSize, 
            cancellationToken);

        return result;  
    }
}