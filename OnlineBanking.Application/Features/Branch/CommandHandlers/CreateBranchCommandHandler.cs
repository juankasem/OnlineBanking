
using AutoMapper;
using OnlineBanking.Application.Features.Branch.Commands;
using OnlineBanking.Core.Domain.Aggregates.BranchAggregate;

namespace OnlineBanking.Application.Features.Branch.CommandHandlers;

public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateBranchCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResult<Unit>> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();

        var address = _mapper.Map<Address>(request.Address);

        var branch = CreateBranch(request);
        branch.SetAddress(address);

        await _uow.Branches.AddAsync(branch);
        await _uow.SaveAsync();

        return result;
    }

    #region Private helper methods
    private Core.Domain.Aggregates.BranchAggregate.Branch CreateBranch(CreateBranchCommand request) =>
        Core.Domain.Aggregates.BranchAggregate.
            Branch.Create(request.Name);
    #endregion
}
