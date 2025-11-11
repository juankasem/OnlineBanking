using AutoMapper;
using OnlineBanking.Application.Features.BankAccounts.Commands;

namespace OnlineBanking.Application.Features.BankAccounts.CommandHandlers;

public class ActivateBankAccountCommandHandler : IRequestHandler<ActivateBankAccountCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public ActivateBankAccountCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResult<Unit>> Handle(ActivateBankAccountCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();

        var bankAccount = await _uow.BankAccounts.GetByIdAsync(request.BankAccountId);
        if (bankAccount is null)
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, "Id", request.BankAccountId));

            return result;
        }

        bankAccount.Activate();

        _uow.BankAccounts.Update(bankAccount);
        await _uow.SaveAsync();

        return result;
    }
}