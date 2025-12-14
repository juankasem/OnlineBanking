using OnlineBanking.Application.Models.Branch.Responses;

namespace OnlineBanking.Application.Features.Branch.GetAll;

public class GetAllBranchesRequest : IRequest<ApiResult<PagedList<BranchResponse>>>
{
    public BranchParams BranchParams { get; set; }
}