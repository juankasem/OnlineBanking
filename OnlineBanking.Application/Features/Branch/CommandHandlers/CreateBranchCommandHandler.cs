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
using OnlineBanking.Core.Domain.Aggregates.AddressAggregate;
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
        var validator = new CreateBranchCommandValidator();

        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            validationResult.Errors.ForEach(er => result.AddError(ErrorCode.ValidationError, er.ErrorMessage));

            return result;
        }

        try
        {
            var address = _mapper.Map<Address>(request.Address);

            var branch = CreateBranch(request, address);

            await _uow.Branches.AddAsync(branch);

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
    private  OnlineBanking.Core.Domain.Aggregates.BranchAggregate.Branch CreateBranch(CreateBranchCommand request, Address address) =>
        OnlineBanking.Core.Domain.Aggregates.BranchAggregate.
            Branch.Create(request.Name);
    #endregion
}
