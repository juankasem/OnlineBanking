using OnlineBanking.Application.Extensions;

namespace OnlineBanking.Application.Features.BankAccounts.GetAll;

/// <summary>
/// Handles requests to retrieve all bank accounts with pagination.
/// Maps domain entities to DTOs and returns paginated results.
/// </summary>
public class GetAllBankAccountsRequestHandler(
    IUnitOfWork uow, 
    IBankAccountMapper bankAccountMapper,
    ILogger<GetAllBankAccountsRequestHandler> logger) : 
    IRequestHandler<GetAllBankAccountsRequest, ApiResult<PagedList<BankAccountDto>>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IBankAccountMapper _bankAccountMapper = bankAccountMapper;
    private readonly ILogger<GetAllBankAccountsRequestHandler> _logger = logger;

    /// <summary>
    /// Handles the request to retrieve all bank accounts with pagination.
    /// </summary>
    public async Task<ApiResult<PagedList<BankAccountDto>>> Handle(
        GetAllBankAccountsRequest request, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.BankAccountParams);

        var result = new ApiResult<PagedList<BankAccountDto>>();
        var bankAccountParams = request.BankAccountParams;

        _logger.LogInformation(
        "Retrieving all bank accounts - Page: {Page}, Size: {Size}",
        bankAccountParams.PageNumber,
        request.BankAccountParams.PageSize);

        // Retrieve bank accounts from repository
        var (bankAccounts, totalCount) = await _uow.BankAccounts.GetAllBankAccountsAsync(bankAccountParams);

        // Handle empty result
        if (bankAccounts.Count == 0)
        {
            _logger.LogInformation(
                "No bank accounts found for page {Page}", 
                bankAccountParams.PageNumber);
  
            result.Payload = PagedList<BankAccountDto>.Create([], 0, 0, 0);
            return result;
        }

        // Map bank accounts to DTOs
        var mappedBankAccounts = bankAccounts
            .Select(bankAccount => _bankAccountMapper.MapToDtoModel(bankAccount))
            .ToList()
            .AsReadOnly();

        // Create paginated response
        result.Payload = mappedBankAccounts.ToPagedList(
            totalCount, 
            bankAccountParams.PageNumber, 
            bankAccountParams.PageSize,
            cancellationToken);

        _logger.LogInformation(
            "Retrieved {AccountCount} of {TotalCount} bank accounts",
            bankAccounts.Count,
            totalCount);

        return result;
    }
}