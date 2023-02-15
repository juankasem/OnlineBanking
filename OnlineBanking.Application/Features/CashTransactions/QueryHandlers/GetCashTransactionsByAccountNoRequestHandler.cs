
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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

public class GetCashTransactionsByAccountNoRequestHandler : IRequestHandler<GetCashTransactionsByAccountNoRequest, ApiResult<PagedList<CashTransactionResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICashTransactionsMapper _cashTransactionsMapper;
    public GetCashTransactionsByAccountNoRequestHandler(IUnitOfWork uow, 
                                                        IMapper mapper,
                                                        ICashTransactionsMapper cashTransactionsMapper)
    {
        _uow = uow;
        _mapper = mapper;
        _cashTransactionsMapper = cashTransactionsMapper;
    }

    public async Task<ApiResult<PagedList<CashTransactionResponse>>> Handle(GetCashTransactionsByAccountNoRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedList<CashTransactionResponse>>();

        if (!await _uow.BankAccounts.ExistsAsync(request.AccountNo))
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, "No.", request.AccountNo));

            return result;
        }
        var requestParams = request.CashTransactionParams;
        var accountTransactions = await _uow.CashTransactions.GetByAccountNoAsync(request.AccountNo, requestParams);

        if (!accountTransactions.Any())
        {
            return result;
        }

        var mappedAccountTransactions = accountTransactions.Select(act => _cashTransactionsMapper.MapToResponseModel(act, request.AccountNo))
                                                           .ToImmutableList();

        result.Payload = PagedList<CashTransactionResponse>.CreateAsync(mappedAccountTransactions, requestParams.PageNumber, requestParams.PageSize);

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
