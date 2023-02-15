using OnlineBanking.Application.Models.Branch.Responses;
using OnlineBanking.Core.Domain.Aggregates.BranchAggregate;

namespace OnlineBanking.Application.Mappings.Branches;

public interface IBranchMapper
{
  BranchResponse MapToResponseModel(Branch branch);
}