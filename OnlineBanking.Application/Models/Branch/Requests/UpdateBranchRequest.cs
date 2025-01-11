using OnlineBanking.Application.Models.Address.Base;

namespace OnlineBanking.Application.Models.Branch.Requests;

public class UpdateBranchRequest
{
    public string BranchName { get; set; }
    public BaseAddressDto BranchAddress { get; set; }
}
