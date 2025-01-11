
using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.Branch.Responses;
using OnlineBanking.Core.Helpers;
using OnlineBanking.Core.Helpers.Params;

namespace OnlineBanking.Application.Features.Branch.Queries;

public class GetAllBranchesRequest : IRequest<ApiResult<PagedList<BranchResponse>>>
{
 public BranchParams BranchParams { get; set; }
}