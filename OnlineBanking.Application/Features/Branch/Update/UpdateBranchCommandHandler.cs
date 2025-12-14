
using OnlineBanking.Application.Features.Branch.Messages;
using OnlineBanking.Core.Domain.Aggregates.BranchAggregate;

namespace OnlineBanking.Application.Features.Branch.Update;

public class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateBranchCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResult<Unit>> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();

        var branch = await _uow.Branches.GetByIdAsync(request.BranchId);

        if (branch is null)
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(BranchErrorMessages.NotFound, "IBAN", request.BranchId));

            return result;
        }

        var address = _mapper.Map<Address>(request.BranchAddress);

        branch.SetAddress(address);

        _uow.Branches.Update(branch);

        return result;
    }
}