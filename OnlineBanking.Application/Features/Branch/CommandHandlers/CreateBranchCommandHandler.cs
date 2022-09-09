using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Features.Branch.Commands;
using OnlineBanking.Application.Features.Branch.Validators;
using OnlineBanking.Application.Models;

namespace OnlineBanking.Application.Features.Branch.CommandHandlers
{
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
            var validator = new CreateBranchCommandValidator(_uow);

            var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            validationResult.Errors.ForEach(er => result.AddError(ErrorCode.ValidationError, er.ErrorMessage));

            return result;
        }

            try
            {
                var bankAccount = CreateBankAccount(request);

                await _uow.BankAccounts.AddAsync(bankAccount);

                return result;
            }
            catch (BankAccountNotValidException e)
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
}