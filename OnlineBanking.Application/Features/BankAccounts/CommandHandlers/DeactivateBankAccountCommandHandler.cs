using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts;
using OnlineBanking.Application.Features.BankAccounts.Commands;
using OnlineBanking.Application.Models;

namespace OnlineBanking.Application.Features.BankAccount.CommandHandlers;
public class DeactivateBankAccountCommandHandler : IRequestHandler<DeactivateBankAccountCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public DeactivateBankAccountCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }
    public async Task<ApiResult<Unit>> Handle(DeactivateBankAccountCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();

        var bankAccount = await _uow.BankAccounts.GetByIdAsync(request.BankAccountId);
        if (bankAccount is null)
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, "Id", request.BankAccountId));

            return result;
        }

        bankAccount.Deactivate();

        _uow.BankAccounts.Add(bankAccount);    
        await _uow.SaveAsync();

        return result;
    }
}
