
using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.Branch.Responses;
using OnlineBanking.Core.Helpers.Params;

namespace OnlineBanking.Application.Features.Branch.Queries;

public class GetAllBranchesRequest : IRequest<ApiResult<BranchListResponse>>
{
 public BranchParams BranchParams { get; set; }

}