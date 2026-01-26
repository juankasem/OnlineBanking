
namespace OnlineBanking.Application.Features.BankAccounts.GetWithTransactions;

public class GetBankAccountWithTransactionsRequest : IRequest<ApiResult<BankAccountResponse>>
{
    public string IBAN { get; set; }
    public CashTransactionParams AccountTransactionsParams { get; set; }
}