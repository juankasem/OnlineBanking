namespace OnlineBanking.Application.Models.Branch;

public class BranchDto
{
    public int Id { get; set; }
    public string BranchName { get; set; }

    private BranchDto()
    {
    }

    public BranchDto(int id, string branchName)
    {
        Id = id;
        BranchName = branchName;
    }
}