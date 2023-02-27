using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.CashTransactions.Commands;
using OnlineBanking.Application.Models;

namespace OnlineBanking.Application.Features.CashTransactions.CommandHandlers;

public class DeleteCashTransactionCommandHandler : IRequestHandler<DeleteCashTransactionCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public DeleteCashTransactionCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }
    
    public async Task<ApiResult<Unit>> Handle(DeleteCashTransactionCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();

        try
        {
            var cashTransaction = await _uow.CashTransactions.GetByIdAsync(request.Id);

            if (cashTransaction is null)
            {
                result.AddError(ErrorCode.NotFound,
                string.Format(CashTransactionErrorMessages.NotFound, request.Id));

                return result;
            }

            _uow.CashTransactions.Delete(cashTransaction);
            await _uow.SaveAsync();

            return result;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }
}