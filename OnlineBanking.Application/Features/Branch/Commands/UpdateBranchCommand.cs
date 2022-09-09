using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.Address;

namespace OnlineBanking.Application.Features.Branch.Commands;

public class UpdateBranchCommand : IRequest<ApiResult<Unit>>
{
    public int BranchId { get; set; }
    public string Name { get; set; }
    public AddressDto Address { get; set; }
}