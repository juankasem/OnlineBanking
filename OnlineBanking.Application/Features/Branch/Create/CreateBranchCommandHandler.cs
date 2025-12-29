using OnlineBanking.Core.Domain.Aggregates.BranchAggregate;

namespace OnlineBanking.Application.Features.Branch.Create;

public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateBranchCommandHandler> _logger;

    public CreateBranchCommandHandler(IUnitOfWork uow, 
                                    IMapper mapper, 
                                    ILogger<CreateBranchCommandHandler> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResult<Unit>> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();
        var address = _mapper.Map<Address>(request.Address);

        var branch = CreateBranch(request);
        branch.SetAddress(address);

        await _uow.Branches.AddAsync(branch);

        // Persist changes
        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            _logger.LogInformation("Branch of id: {branchId}, name: {name} is created successfully.",
                                   branch.Id,
                                   branch.Name);
        }
        else
        {
            result.AddError(ErrorCode.UnknownError, BankAccountErrorMessages.Unknown);
            _logger.LogError($"Creating a new bank account failed!");
        }

        return result;
    }

    #region Private helper methods

    private static Core.Domain.Aggregates.BranchAggregate.Branch CreateBranch(CreateBranchCommand request) =>
        Core.Domain.Aggregates.BranchAggregate.
            Branch.Create(request.Name);
    #endregion
}
