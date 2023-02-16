using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.Branch.Commands;
using OnlineBanking.Application.Features.Branch.Messages;
using OnlineBanking.Application.Features.Branch.Validators;
using OnlineBanking.Application.Models;
using OnlineBanking.Core.Domain.Aggregates.AddressAggregate;
using OnlineBanking.Core.Domain.Exceptions;

namespace OnlineBanking.Application.Features.Branch.CommandHandlers;

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
        var validator = new UpdateBranchCommandValidator();

        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            validationResult.Errors.ForEach(er => result.AddError(ErrorCode.ValidationError, er.ErrorMessage));

            return result;
        }

        try
        {
            var branch = await _uow.Branches.GetByIdAsync(request.BranchId);

            if (branch is null)
            {
                result.AddError(ErrorCode.NotFound,
                string.Format(BranchErrorMessages.NotFound, "IBAN", request.BranchId));

                return result;
            }

            var address = _mapper.Map<Address>(request.Address);

            branch.SetAddress(address);

            await _uow.Branches.UpdateAsync(branch);

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
}