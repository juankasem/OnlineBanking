using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Extensions;
using OnlineBanking.Application.Features.CashTransactions.Queries;
using OnlineBanking.Application.Helpers;
using OnlineBanking.Application.Mappings.CashTransactions;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CashTransaction.Responses;

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

        var mappedCashTransactions = cashTransactions.Select(ct => _cashTransactionsMapper.MapToResponseModel(ct, ct.From))
                                                     .ToList().AsReadOnly();
                                                     
        result.Payload = mappedCashTransactions.ToPagedList(totalCount, cashTransactionParams.PageNumber, cashTransactionParams.PageSize);

        return result;
    }
}