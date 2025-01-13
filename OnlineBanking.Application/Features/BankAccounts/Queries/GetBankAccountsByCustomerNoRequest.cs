using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.BankAccount.Responses;
using OnlineBanking.Core.Helpers;
using OnlineBanking.Core.Helpers.Params;

namespace OnlineBanking.Application.Features.BankAccounts.Queries;

public class GetBankAccountsByCustomerNoRequest : IRequest<ApiResult<PagedList<BankAccountResponse>>>
{
    public string CustomerNo { get; set; }
    public BankAccountParams BankAccountParams { get; set; }
    public CashTransactionParams AccountTransactionsParams { get; set; }
}