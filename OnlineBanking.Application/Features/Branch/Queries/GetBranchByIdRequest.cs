using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.Branch.Responses;

namespace OnlineBanking.Application.Features.Branch.Queries;

public class GetBranchByIdRequest : IRequest<ApiResult<BranchResponse>>
{
    public int BranchId { get; set; }
}

