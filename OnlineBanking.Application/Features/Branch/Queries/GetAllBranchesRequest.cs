
using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.Branch.Responses;

namespace OnlineBanking.Application.Features.Branch.Queries;

public class GetAllBranchesRequest : IRequest<ApiResult<BranchListResponse>>
{
}