using System.Collections.Immutable;

namespace OnlineBanking.Application.Models.Branch.Responses;

public class BranchListResponse
{
    public ImmutableList<BranchResponse> Branches { get; set; }
    public int Count { get; set; }

    public BranchListResponse(ImmutableList<BranchResponse> branches, int count)
    {
        Branches = branches;
        Count = count;
    }
}