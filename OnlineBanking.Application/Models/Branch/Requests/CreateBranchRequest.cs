using OnlineBanking.Application.Models.Address.Base;

namespace OnlineBanking.Application.Models.Branch.Requests;

public class CreateBranchRequest
{
    public int Name { get; set; }
    public BaseAddressDto Address { get; set; }
}