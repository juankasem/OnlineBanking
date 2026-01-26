using OnlineBanking.Application.Extensions;
using OnlineBanking.Application.Models.Branch.Responses;

namespace OnlineBanking.Application.Features.Branch.GetAll;

public class GetAllBranchesRequestHandler : 
    IRequestHandler<GetAllBranchesRequest, ApiResult<PagedList<BranchResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IBranchMapper _branchMapper;
    public GetAllBranchesRequestHandler(IUnitOfWork uow, 
                                        IMapper mapper,
                                        IBranchMapper branchMapper)
    {
        _uow = uow;
        _mapper = mapper;
        _branchMapper = branchMapper;
    }

    public async Task<ApiResult<PagedList<BranchResponse>>> Handle(
        GetAllBranchesRequest request, 
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedList<BranchResponse>>();
        var branchParams = request.BranchParams;

        var (branches, totalCount) = await _uow.Branches.GetAllAsync(request.BranchParams);

        var mappedBranches = _mapper.Map<IReadOnlyList<BranchResponse>>(branches);

        result.Payload = mappedBranches.ToPagedList(
            totalCount, 
            branchParams.PageNumber, 
            branchParams.PageSize, 
            cancellationToken);

        return result;
    }
}