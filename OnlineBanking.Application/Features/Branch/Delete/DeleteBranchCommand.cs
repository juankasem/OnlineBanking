namespace OnlineBanking.Application.Features.Branch.Delete;

public class DeleteBranchCommand : IRequest<ApiResult<Unit>>
{
    public int BranchId { get; set; }
}
