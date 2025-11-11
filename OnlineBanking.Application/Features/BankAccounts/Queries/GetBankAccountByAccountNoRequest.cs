
namespace OnlineBanking.Application.Features.BankAccounts.Queries;

public class GetBankAccountByAccountNoRequest : IRequest<ApiResult<BankAccountResponse>>
{
    public string AccountNo { get; set; }
    public CashTransactionParams AccountTransactionsParams { get; set; }
}