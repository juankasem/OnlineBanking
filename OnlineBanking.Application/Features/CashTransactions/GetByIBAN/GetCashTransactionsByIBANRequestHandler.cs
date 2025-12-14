using OnlineBanking.Application.Extensions;
using OnlineBanking.Application.Mappings.CashTransactions;
using OnlineBanking.Application.Models.CashTransaction.Responses;

namespace OnlineBanking.Application.Features.CashTransactions.GetByIBAN;

public class GetCashTransactionsByAccountNoOrIBANRequestHandler : IRequestHandler<GetCashTransactionsByAccountNoOrIBANRequest, ApiResult<PagedList<CashTransactionResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICashTransactionsMapper _cashTransactionsMapper;

    public GetCashTransactionsByAccountNoOrIBANRequestHandler(IUnitOfWork uow, ICashTransactionsMapper cashTransactionsMapper)
    {
        _uow = uow;
        _cashTransactionsMapper = cashTransactionsMapper;
    }

    public async Task<ApiResult<PagedList<CashTransactionResponse>>> Handle(GetCashTransactionsByAccountNoOrIBANRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedList<CashTransactionResponse>>();

        if (!await _uow.BankAccounts.ExistsAsync(request.IBAN))
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.IBAN));

            return result;
        }
        var cashTransactionParams = request.CashTransactionParams;
        var (accountTransactions, totalCount) = await _uow.CashTransactions.GetByAccountNoOrIBANAsync(request.IBAN, cashTransactionParams);

        if (!accountTransactions.Any())
        {
            return result;
        }

        var mappedAccountTransactions = accountTransactions.Select(at => _cashTransactionsMapper.MapToResponseModel(at, request.IBAN))
                                                            .ToList().AsReadOnly();

        result.Payload = mappedAccountTransactions.ToPagedList(totalCount, cashTransactionParams.PageNumber, cashTransactionParams.PageSize);

        return result;
    }
}