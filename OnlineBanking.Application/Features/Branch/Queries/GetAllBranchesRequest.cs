using OnlineBanking.Application.Helpers.Params;
using OnlineBanking.Application.Models.Branch.Responses;

namespace OnlineBanking.Application.Features.Branch.Queries;

public class GetAllBranchesRequest : IRequest<ApiResult<PagedList<BranchResponse>>>
{
    public BranchParams BranchParams { get; set; }
}