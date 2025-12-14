using AutoMapper;

namespace OnlineBanking.Application.Features.CashTransactions.Update;

public class UpdateCashTransactionCommandHandler : IRequestHandler<UpdateCashTransactionCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public UpdateCashTransactionCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }
    public async Task<ApiResult<Unit>> Handle(UpdateCashTransactionCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();

        var cashTransaction = await _uow.CashTransactions.GetByIdAsync(request.Id);

        if (cashTransaction is null)
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(CashTransactionErrorMessages.NotFound, request.Id));

            return result;
        }

        _uow.CashTransactions.Update(cashTransaction);
        await _uow.SaveAsync();

        return result;
    }
}