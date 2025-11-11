using OnlineBanking.Application.Models.Branch;

namespace OnlineBanking.Application.Features.Branch.Commands;

public class CreateBranchCommand : IRequest<ApiResult<Unit>>
{
    public string Name { get; set; }
    public BranchAddressDto Address { get; set; }
}