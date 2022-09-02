using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.CashTransactions.Commands;
using OnlineBanking.Application.Models;

namespace OnlineBanking.Application.Features.CashTransactions.CommandHandlers;

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

        cashTransaction.Update(request.BaseCasTransaction.Status);

        return result;
    }
}