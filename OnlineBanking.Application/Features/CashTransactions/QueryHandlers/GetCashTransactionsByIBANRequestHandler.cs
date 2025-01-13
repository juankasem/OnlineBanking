using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts;
using OnlineBanking.Application.Features.CashTransactions.Queries;
using OnlineBanking.Application.Mappings.CashTransactions;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CashTransaction.Responses;
using OnlineBanking.Core.Helpers;

namespace OnlineBanking.Application.Features.CashTransactions.QueryHandlers;

public class GetCashTransactionsByIBANRequestHandler : IRequestHandler<GetCashTransactionsByIBANRequest, ApiResult<PagedList<CashTransactionResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICashTransactionsMapper _cashTransactionsMapper;

    public GetCashTransactionsByIBANRequestHandler(IUnitOfWork uow, ICashTransactionsMapper cashTransactionsMapper)
    {
        _uow = uow;
        _cashTransactionsMapper = cashTransactionsMapper;
    }
    
    public async Task<ApiResult<PagedList<CashTransactionResponse>>> Handle(GetCashTransactionsByIBANRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedList<CashTransactionResponse>>();

        if (!await _uow.BankAccounts.ExistsAsync(request.IBAN))
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.IBAN));

            return result;
        }
        var cashTransactionParams = request.CashTransactionParams;
        var (accountTransactions, totalCount) = await _uow.CashTransactions.GetByIBANAsync(request.IBAN, cashTransactionParams);

        if (!accountTransactions.Any())
        {
            return result;
        }

        var mappedAccountTransactions = accountTransactions.Select(at => _cashTransactionsMapper.MapToResponseModel(at, request.IBAN))
                                                            .ToList().AsReadOnly();

        result.Payload = PagedList<CashTransactionResponse>.Create(mappedAccountTransactions, totalCount, 
                                                                   cashTransactionParams.PageNumber, cashTransactionParams.PageSize);
                                           ;
    
        return result;
    }
}