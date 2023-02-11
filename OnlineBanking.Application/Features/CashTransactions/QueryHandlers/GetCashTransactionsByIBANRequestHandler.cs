using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts;
using OnlineBanking.Application.Features.CashTransactions.Queries;
using OnlineBanking.Application.Mappings.CashTransactions;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CashTransaction.Responses;

namespace OnlineBanking.Application.Features.CashTransactions.QueryHandlers;

public class GetCashTransactionsByIBANRequestHandler : IRequestHandler<GetCashTransactionsByIBANRequest, ApiResult<CashTransactionListResponse>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICashTransactionsMapper _cashTransactionsMapper;

    public GetCashTransactionsByIBANRequestHandler(IUnitOfWork uow, ICashTransactionsMapper cashTransactionsMapper)
    {
        _uow = uow;
        _cashTransactionsMapper = cashTransactionsMapper;
    }
    
    public async Task<ApiResult<CashTransactionListResponse>> Handle(GetCashTransactionsByIBANRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<CashTransactionListResponse>();

        if (!await _uow.BankAccounts.ExistsAsync(request.IBAN))
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.IBAN));

            return result;
        }

        var accountTransactions = await _uow.CashTransactions.GetByIBANAsync(request.IBAN);

        if (!accountTransactions.Any())
        {
            return result;
        }

        var mappedAccountTransactions = accountTransactions.Select(act => _cashTransactionsMapper.MapToResponseModel(act, request.IBAN))
                                                            .ToImmutableList();

        result.Payload = new(mappedAccountTransactions, mappedAccountTransactions.Count);

        return result;
    }

    private async Task<string> GetAccountOwnerName(string iban)
    {
        var customer = await _uow.Customers.GetByIBANAsync(iban);
        return customer != null ? customer.FirstName + " " + customer.LastName : string.Empty;
    }
}