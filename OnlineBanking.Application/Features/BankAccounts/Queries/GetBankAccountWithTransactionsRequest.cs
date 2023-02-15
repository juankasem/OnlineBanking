using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.BankAccount.Responses;
using OnlineBanking.Core.Helpers.Params;

namespace OnlineBanking.Application.Features.BankAccounts.Queries;

public class GetBankAccountWithTransactionsRequest : IRequest<ApiResult<BankAccountResponse>>
{
    public string IBAN { get; set; }
    public CashTransactionParams AccountTransactionsParams { get; set; }
}