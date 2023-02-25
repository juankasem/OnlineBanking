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
        var reqParams = request.CashTransactionParams;
        var accountTransactions = await _uow.CashTransactions.GetByAccountNoOrIBANAsync(request.AccountNoOrIBAN, reqParams);

        if (!accountTransactions.Any())
        {
            return result;
        }

        var mappedAccountTransactions = accountTransactions.Select(act => _cashTransactionsMapper.MapToResponseModel(act, request.AccountNoOrIBAN))
                                                           .ToList()
                                                           .AsReadOnly();

        result.Payload = PagedList<CashTransactionResponse>.Create(mappedAccountTransactions, reqParams.PageNumber, reqParams.PageSize);

        return result;
    }

    #region Private Helpermethods
    private async Task<string> GetAccountOwnerName(string accountNo)
    {
        var customer = await _uow.Customers.GetByCustomerNoAsync(accountNo);
        return customer != null ? customer.FirstName + " " + customer.LastName : string.Empty;
    }
    #endregion
}
