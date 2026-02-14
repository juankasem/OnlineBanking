using OnlineBanking.Application.Extensions;
using OnlineBanking.Application.Models.CashTransaction.Responses;

namespace OnlineBanking.Application.Features.CashTransactions.GetByAccountNoOrIBAN;

/// <summary>
/// Handles requests to retrieve cash transactions for a specific bank account.
/// Retrieves paginated transaction history and maps to response models.
/// </summary>
public class GetCashTransactionsByAccountNoOrIBANRequestHandler(
    IUnitOfWork uow,
    ICashTransactionsMapper cashTransactionsMapper,
    ILogger<GetCashTransactionsByAccountNoOrIBANRequestHandler> logger) : 
    IRequestHandler<GetCashTransactionsByAccountNoOrIBANRequest, ApiResult<PagedList<CashTransactionResponse>>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ICashTransactionsMapper _cashTransactionsMapper = cashTransactionsMapper;
    private readonly ILogger<GetCashTransactionsByAccountNoOrIBANRequestHandler> _logger = logger;

    /// <summary>
    /// Handles the request to retrieve transactions for an account.
    /// </summary>
    public async Task<ApiResult<PagedList<CashTransactionResponse>>> Handle(
        GetCashTransactionsByAccountNoOrIBANRequest request, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.AccountNoOrIBAN);
        ArgumentNullException.ThrowIfNull(request.CashTransactionParams);

        var result = new ApiResult<PagedList<CashTransactionResponse>>();
        var accountIdentifier = request.AccountNoOrIBAN;
        var cashTransactionParams = request.CashTransactionParams;

        _logger.LogInformation(
            "Retrieving transactions for account: {AccountIdentifier}", 
            accountIdentifier);

        if (!await _uow.BankAccounts.ExistsAsync(accountIdentifier))
        {
            _logger.LogWarning(
                "Account validation failed: Account {AccountIdentifier} not found",
                accountIdentifier);

            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, 
            "Account No/IBAN", 
            accountIdentifier));
            return result;
        }

        // Retrieve cash transactions
        var (cashTransactions, totalCount) = await _uow.CashTransactions.GetByAccountNoOrIBANAsync(
            accountIdentifier, 
            cashTransactionParams);

        // Handle empty result
        if (cashTransactions.Count == 0)
        {
            _logger.LogInformation("No transactions found for account: {AccountIdentifier}", 
                accountIdentifier);

            result.Payload = PagedList<CashTransactionResponse>.Create([], 0, 0, 0);
            return result;
        }

        // Map cash transactions to response models
        var mappedCashTransactions = cashTransactions
            .Select(ct => _cashTransactionsMapper.MapToResponseModel(ct, accountIdentifier))
            .ToList()
            .AsReadOnly();

        // Create paginated response
        result.Payload = mappedCashTransactions.ToPagedList(totalCount, 
            cashTransactionParams.PageNumber, 
            cashTransactionParams.PageSize, 
            cancellationToken);

        _logger.LogInformation(
            "Retrieved {TransactionCount} of {TotalCount} transactions for account: {AccountIdentifier}",
            cashTransactions.Count,
            totalCount,
            accountIdentifier);

        return result;  
    }
}