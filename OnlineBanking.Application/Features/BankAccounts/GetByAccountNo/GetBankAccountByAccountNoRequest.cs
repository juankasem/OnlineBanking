namespace OnlineBanking.Application.Features.BankAccounts.GetByAccountNo;

public class GetBankAccountByAccountNoRequest : IRequest<ApiResult<BankAccountResponse>>
{
    public string AccountNo { get; set; }
    public CashTransactionParams AccountTransactionsParams { get; set; }
}