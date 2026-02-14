using OnlineBanking.Application.Extensions;
using OnlineBanking.Application.Models.Branch.Responses;

namespace OnlineBanking.Application.Features.Branch.GetAll;

/// <summary>
/// Handles requests to retrieve all branches with pagination.
/// Maps domain entities to response models and returns paginated results.
/// </summary>
public class GetAllBranchesRequestHandler(
    IUnitOfWork uow,
    IMapper mapper,
    ILogger<GetAllBranchesRequestHandler> logger) : 
    IRequestHandler<GetAllBranchesRequest, ApiResult<PagedList<BranchResponse>>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<GetAllBranchesRequestHandler> _logger = logger;

    /// <summary>
    /// Handles the request to retrieve all branches with pagination.
    /// </summary>
    public async Task<ApiResult<PagedList<BranchResponse>>> Handle(
        GetAllBranchesRequest request, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.BranchParams);

        var result = new ApiResult<PagedList<BranchResponse>>();
        var branchParams = request.BranchParams;

        _logger.LogInformation(
            "Retrieving all branches - Page: {Page}, Size: {Size}",
            branchParams.PageNumber,
            branchParams.PageSize);

        // Retrieve branches from repository
        var (branches, totalCount) = await _uow.Branches.GetAllAsync(branchParams);

        // Handle empty result
        if (branches.Count == 0)
        {
            _logger.LogInformation(
                "No branches found for page {Page}",
                branchParams.PageNumber);

            result.Payload = PagedList<BranchResponse>.Create([], 0, 0, 0);
            return result;
        }

        // Map branches to response models
        var mappedBranches = _mapper.Map<IReadOnlyList<BranchResponse>>(branches);

        // Create paginated response
        result.Payload = mappedBranches.ToPagedList(
            totalCount, 
            branchParams.PageNumber, 
            branchParams.PageSize, 
            cancellationToken);

        _logger.LogInformation(
            "Retrieved {BranchCount} of {TotalCount} branches",
            branches.Count,
            totalCount);

        return result;
    }
}