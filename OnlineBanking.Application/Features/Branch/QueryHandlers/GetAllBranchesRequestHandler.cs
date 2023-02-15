using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Features.Branch.Queries;
using OnlineBanking.Application.Mappings.Branches;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.Branch.Responses;
using OnlineBanking.Core.Helpers;

namespace OnlineBanking.Application.Features.Branch.QueryHandlers;

public class GetAllBranchesRequestHandler : IRequestHandler<GetAllBranchesRequest, ApiResult<PagedList<BranchResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IBranchMapper _branchMapper;
    public GetAllBranchesRequestHandler(IUnitOfWork uow, IMapper mapper,
                                        IBranchMapper branchMapper)
    {
        _uow = uow;
        _mapper = mapper;
        _branchMapper = branchMapper;
    }

    public async Task<ApiResult<PagedList<BranchResponse>>> Handle(GetAllBranchesRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedList<BranchResponse>>();
        var reqParams = request.BranchParams;

        var allBranches = await _uow.Branches.GetAllAsync(request.BranchParams);

        var mappedBranches = allBranches.Select(branch => _branchMapper.MapToResponseModel(branch))
                                        .ToImmutableList();

        result.Payload = PagedList<BranchResponse>.CreateAsync(mappedBranches, reqParams.PageNumber, reqParams.PageSize);

        return result;
    }
}
