using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.Address.Base;

namespace OnlineBanking.Application.Features.Branch.Commands;

public class CreateBranchCommand : IRequest<ApiResult<Unit>>
{
    public string Name { get; set; }
    public BaseAddressDto Address { get; set; }
}
