using OnlineBanking.Application.Models.Branch.Responses;

namespace OnlineBanking.Application.Features.Branch.GetById;

public class GetBranchByIdRequest : IRequest<ApiResult<BranchResponse>>
{
    public int BranchId { get; set; }
}

