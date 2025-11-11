using OnlineBanking.Application.Models.Branch;

namespace OnlineBanking.Application.Features.Branch.Commands;

public class UpdateBranchCommand : IRequest<ApiResult<Unit>>
{
    public int BranchId { get; set; }
    public string BranchName { get; set; }
    public BranchAddressDto BranchAddress { get; set; }
}