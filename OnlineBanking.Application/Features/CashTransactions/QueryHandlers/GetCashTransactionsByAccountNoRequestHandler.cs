using OnlineBanking.Application.Extensions;
using OnlineBanking.Application.Features.CashTransactions.Queries;
using OnlineBanking.Application.Mappings.CashTransactions;
using OnlineBanking.Application.Models.CashTransaction.Responses;

namespace OnlineBanking.Application.Features.CashTransactions.QueryHandlers;

public class GetAccountTransactionsRequestHandler : IRequestHandler<GetAccountTransactionsRequest, ApiResult<PagedList<CashTransactionResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICashTransactionsMapper _cashTransactionsMapper;
    public GetAccountTransactionsRequestHandler(IUnitOfWork uow,
                                                ICashTransactionsMapper cashTransactionsMapper)
    {
        _uow = uow;
        _cashTransactionsMapper = cashTransactionsMapper;
    }

    public async Task<ApiResult<PagedList<CashTransactionResponse>>> Handle(GetAccountTransactionsRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedList<CashTransactionResponse>>();

        if (!await _uow.BankAccounts.ExistsAsync(request.AccountNoOrIBAN))
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, "No.", request.AccountNoOrIBAN));

            return result;
        }
        var cashTransactionParams = request.CashTransactionParams;
        var (accountTransactions, totalCount) = await _uow.CashTransactions.GetByAccountNoOrIBANAsync(request.AccountNoOrIBAN, cashTransactionParams);

        if (!accountTransactions.Any())
        {
            return result;
        }

        var mappedAccountTransactions = accountTransactions.Select(act => _cashTransactionsMapper.MapToResponseModel(act, request.AccountNoOrIBAN))
                                                           .ToList()
                                                           .AsReadOnly();

        result.Payload = mappedAccountTransactions.ToPagedList(totalCount,
                                                               cashTransactionParams.PageNumber,
                                                               cashTransactionParams.PageSize,
                                                               cancellationToken);
        return result;
    }
}