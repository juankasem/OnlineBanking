
using OnlineBanking.Application.Extensions;
using OnlineBanking.Application.Models.CashTransaction.Responses;

namespace OnlineBanking.Application.Features.CashTransactions.GetAll;

/// <summary>
/// Handles requests to retrieve all cash transactions with pagination.
/// Maps domain entities to response models and returns paginated results.
/// </summary>
public class GetAllCashTransactionsRequestHandler(
    IUnitOfWork uow,
    ICashTransactionsMapper cashTransactionsMapper,
    ILogger<GetAllCashTransactionsRequestHandler> logger) : 
    IRequestHandler<GetAllCashTransactionsRequest, ApiResult<PagedList<CashTransactionResponse>>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ICashTransactionsMapper _cashTransactionsMapper = cashTransactionsMapper;
    private readonly ILogger<GetAllCashTransactionsRequestHandler> _logger = logger;

    /// <summary>
    /// Handles the request to retrieve all cash transactions with pagination.
    /// </summary>
    public async Task<ApiResult<PagedList<CashTransactionResponse>>> Handle(
        GetAllCashTransactionsRequest request, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.CashTransactionParams);

        var result = new ApiResult<PagedList<CashTransactionResponse>>();
        var cashTransactionParams = request.CashTransactionParams;

        _logger.LogInformation(
            "Retrieving all cash transactions - Page: {Page}, Size: {Size}", 
            cashTransactionParams.PageNumber,
            cashTransactionParams.PageSize);

        // Retrieve cash transactions from repository
        var (cashTransactions, totalCount) = await _uow.CashTransactions.GetAllAsync(cashTransactionParams);

        // Handle empty result
        if (cashTransactions.Count == 0)
        {
            _logger.LogInformation(
                "No cash transactions found for page {Page}",
                request.CashTransactionParams.PageNumber);

            result.Payload = PagedList<CashTransactionResponse>.Create([], 0, 0, 0);
            return result;
        }

        // Map cash transactions to response models
        var mappedCashTransactions = cashTransactions
            .Select(ct => _cashTransactionsMapper.MapToResponseModel(ct, ct.From))
            .ToList()
            .AsReadOnly();

        // Create paginated response
        result.Payload = mappedCashTransactions.ToPagedList(
            totalCount, 
            cashTransactionParams.PageNumber, 
            cashTransactionParams.PageSize,
            cancellationToken);

        _logger.LogInformation(
            "Retrieved {TransactionCount} of {TotalCount} cash transactions",
            cashTransactions.Count,
            totalCount);

        return result;
    }
}