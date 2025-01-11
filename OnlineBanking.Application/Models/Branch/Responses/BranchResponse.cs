
namespace OnlineBanking.Application.Models.Branch.Responses;

public class BranchResponse
{
    public int BranchId { get; set; }

    public string BranchName { get; set; }

    public BranchAddressDto BranchAddress { get; set; }
}
