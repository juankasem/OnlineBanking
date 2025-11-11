using AutoMapper;
using OnlineBanking.Application.Features.Branch.Messages;
using OnlineBanking.Application.Features.Branch.Queries;
using OnlineBanking.Application.Models.Branch.Responses;

namespace OnlineBanking.Application.Features.Branch.QueryHandlers;

public class GetBranchByIdRequestHandler : IRequestHandler<GetBranchByIdRequest, ApiResult<BranchResponse>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetBranchByIdRequestHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }
    public async Task<ApiResult<BranchResponse>> Handle(GetBranchByIdRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<BranchResponse>();

        var branch = await _uow.Branches.GetByIdAsync(request.BranchId);

        if (branch is null)
        {
            result.AddError(ErrorCode.NotFound,
                string.Format(BranchErrorMessages.NotFound, "Id", request.BranchId));

            return result;
        }

        result.Payload = _mapper.Map<BranchResponse>(branch);

        return result;
    }
}
