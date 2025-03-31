using MediatR;
using OnlineBanking.Application.Helpers.Params;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.BankAccount.Responses;

namespace OnlineBanking.Application.Features.BankAccounts.Queries;

public class GetBankAccountWithTransactionsRequest : IRequest<ApiResult<BankAccountResponse>>
{
    public string IBAN { get; set; }
    public CashTransactionParams AccountTransactionsParams { get; set; }
}