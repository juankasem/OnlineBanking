using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.Branch;

namespace OnlineBanking.Application.Features.Branch.Commands;

public class UpdateBranchCommand : IRequest<ApiResult<Unit>>
{
    public int BranchId { get; set; }
    public string Name { get; set; }
    public BranchAddressDto Address { get; set; }
}