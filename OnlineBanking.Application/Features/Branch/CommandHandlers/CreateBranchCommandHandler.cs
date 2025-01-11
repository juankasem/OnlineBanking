using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.Branch.Commands;
using OnlineBanking.Application.Features.Branch.Validators;
using OnlineBanking.Application.Models;
using OnlineBanking.Core.Domain.Aggregates.BranchAggregate;
using OnlineBanking.Core.Domain.Exceptions;

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
        
        try
        {
            var address = _mapper.Map<Address>(request.Address);

            var branch = CreateBranch(request);
            branch.SetAddress(address);

            _uow.Branches.Add(branch);
            await _uow.SaveAsync();

            return result;
        }
        catch (BranchNotValidException e)
        {
            e.ValidationErrors.ForEach(er => result.AddError(ErrorCode.ValidationError, er));
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    #region Private helper methods
    private  Core.Domain.Aggregates.BranchAggregate.Branch CreateBranch(CreateBranchCommand request) =>
        Core.Domain.Aggregates.BranchAggregate.
            Branch.Create(request.Name);
    #endregion
}
