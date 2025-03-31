using MediatR;
using OnlineBanking.Application.Helpers;
using OnlineBanking.Application.Helpers.Params;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.BankAccount.Responses;

namespace OnlineBanking.Application.Features.BankAccounts.Queries;

public class GetBankAccountsByCustomerNoRequest : IRequest<ApiResult<PagedList<BankAccountResponse>>>
{
    public string CustomerNo { get; set; }
    public BankAccountParams BankAccountParams { get; set; }
    public CashTransactionParams AccountTransactionsParams { get; set; }
}