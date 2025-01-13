using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Features.CashTransactions.Queries;
using OnlineBanking.Application.Mappings.CashTransactions;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CashTransaction.Responses;
using OnlineBanking.Core.Helpers;

namespace OnlineBanking.Application.Features.CashTransactions.QueryHandlers;

public class GetAllCashTransactionsRequestHandler : IRequestHandler<GetAllCashTransactionsRequest, ApiResult<PagedList<CashTransactionResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICashTransactionsMapper _cashTransactionsMapper;
    public GetAllCashTransactionsRequestHandler(IUnitOfWork uow, IMapper mapper,
                                                ICashTransactionsMapper cashTransactionsMapper)
    {
        _uow = uow;
        _mapper = mapper;
        _cashTransactionsMapper = cashTransactionsMapper;
    }
    public async Task<ApiResult<PagedList<CashTransactionResponse>>> Handle(GetAllCashTransactionsRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedList<CashTransactionResponse>>();
        var cashTransactionParams = request.CashTransactionParams;

        var (cashTransactions, totalCount) = await _uow.CashTransactions.GetAllAsync(cashTransactionParams);

        var mappedCashTransactions = cashTransactions.Select(act => _cashTransactionsMapper.MapToResponseModel(act, act.From))
                                                     .ToList()
                                                     .AsReadOnly();

        result.Payload = PagedList<CashTransactionResponse>.Create(mappedCashTransactions,
                                                                   totalCount,
                                                                   cashTransactionParams.PageNumber, 
                                                                   cashTransactionParams.PageSize); 

        return result;
    }
}