using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Features.CashTransactions.Queries;
using OnlineBanking.Application.Mappings.CashTransactions;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CashTransaction.Responses;

namespace OnlineBanking.Application.Features.CashTransactions.QueryHandlers;

public class GetAllCashTransactionsRequestHandler : IRequestHandler<GetAllCashTransactionsRequest, ApiResult<CashTransactionListResponse>>
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
    public async Task<ApiResult<CashTransactionListResponse>> Handle(GetAllCashTransactionsRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<CashTransactionListResponse>();

        var accountCashTransactions = await _uow.CashTransactions.GetAllAsync();

        var mappedCashTransactions = accountCashTransactions.Select(act => _cashTransactionsMapper.MapToResponseModel(act, act.From))
                                                            .ToImmutableList();

        result.Payload = new(mappedCashTransactions, mappedCashTransactions.Count);

        return result;
    }

    #region private helper methods 
    private async Task<string> GetAccountOwnerName(string iban)
    {
        var customer = await _uow.Customers.GetByIBANAsync(iban);
        return customer != null ? customer.FirstName + " " + customer.LastName : string.Empty;
    }
    #endregion
}
