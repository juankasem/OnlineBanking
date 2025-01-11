using OnlineBanking.Application.Models.Address.Base;

namespace OnlineBanking.Application.Models.Branch.Requests;

public class CreateBranchRequest
{
    public string Name { get; set; }
    public BranchAddressDto Address { get; set; }
}