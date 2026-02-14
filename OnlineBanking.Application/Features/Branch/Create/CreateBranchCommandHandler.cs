using OnlineBanking.Core.Domain.Aggregates.BranchAggregate;

namespace OnlineBanking.Application.Features.Branch.Create;

// <summary>
/// Handles branch creation requests.
/// Validates branch data, creates domain entity, and persists changes.
/// </summary>
public class CreateBranchCommandHandler(
    IUnitOfWork uow,
    IMapper mapper,
    ILogger<CreateBranchCommandHandler> logger) : 
    IRequestHandler<CreateBranchCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<CreateBranchCommandHandler> _logger = logger;

    /// <summary>
    /// Handles the branch creation request.
    /// </summary>
    public async Task<ApiResult<Unit>> Handle(
        CreateBranchCommand request, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Name);
        ArgumentNullException.ThrowIfNull(request.Address);

        var result = new ApiResult<Unit>();

        _logger.LogInformation(
            "Processing branch creation request: Name={BranchName}",
            request.Name);

        // Map address DTO to domain entity
        var address = _mapper.Map<Address>(request.Address);

        // Create branch aggregate
        var branch = CreateBranch(request);
        branch.SetAddress(address);

        // Persist branch
        await _uow.Branches.AddAsync(branch);

        // Persist changes
        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            _logger.LogInformation(
                  "Branch created successfully - Id: {BranchId}, Name: {BranchName}",
                  branch.Id,
                  branch.Name);
        }
        else
        {
            _logger.LogError(
                   "Failed to persist branch creation for branch name: {BranchName}. " +
                   "Database transaction returned 0 rows affected",
                   branch.Name);
            result.AddError(ErrorCode.UnknownError, BankAccountErrorMessages.Unknown);
        }

        return result;
    }

    #region Private helper methods

    private static Core.Domain.Aggregates.BranchAggregate.Branch CreateBranch(CreateBranchCommand request) =>
        Core.Domain.Aggregates.BranchAggregate.
            Branch.Create(request.Name);
    #endregion
}
