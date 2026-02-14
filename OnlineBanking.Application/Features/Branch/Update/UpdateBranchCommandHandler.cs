using OnlineBanking.Application.Features.Branch.Messages;
using OnlineBanking.Core.Domain.Aggregates.BranchAggregate;

namespace OnlineBanking.Application.Features.Branch.Update;

// <summary>
/// Handles branch update requests.
/// Validates branch data, creates domain entity, and persists changes.
/// </summary>
public class UpdateBranchCommandHandler(
    IUnitOfWork uow, 
    IMapper mapper,
    ILogger<UpdateBranchCommandHandler> logger) : 
    IRequestHandler<UpdateBranchCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IMapper _mapper = mapper; 
    private readonly ILogger<UpdateBranchCommandHandler> _logger = logger;

    /// <summary>
    /// Handles the branch update request.
    /// </summary>
    public async Task<ApiResult<Unit>> Handle(
        UpdateBranchCommand request, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.BranchName);
        ArgumentNullException.ThrowIfNull(request.BranchAddress);

        var result = new ApiResult<Unit>();
        var branchId = request.BranchId;

        _logger.LogInformation(
            "Processing branch update request: {BranchId}", 
            branchId);

        // Retrieve and validate branch 
        var branch = await _uow.Branches.GetByIdAsync(branchId);
        if (branch is null)
        {
            _logger.LogWarning(
                "Branch validation failed: Branch {BranchId} not found", 
                branchId);

            result.AddError(ErrorCode.NotFound,
            string.Format(BranchErrorMessages.NotFound, branchId));
            return result;
        }

        // Maps address DTO to domain Address entity.
        var address = _mapper.Map<Address>(request.BranchAddress);
        branch.SetAddress(address);

        // Set branch name
        branch.SetName(request.BranchName);

        _uow.Branches.Update(branch);

        // Persist changes
        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            _logger.LogInformation(
                "Branch updated successfully: {BranchId}",
                branchId);
        }
        else
        {
            _logger.LogError(
                "Failed to persist branch update for branch: {BranchId}. " +
                "Database transaction returned 0 rows affected",
                branchId);
            result.AddError(ErrorCode.UnknownError, BankAccountErrorMessages.Unknown);
        }

        return result;
    }
}