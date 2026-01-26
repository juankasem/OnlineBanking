
namespace OnlineBanking.Application.Features.BankAccounts.GetByCustomerNo;

public class GetBankAccountsByCustomerNoRequest : IRequest<ApiResult<PagedList<BankAccountResponse>>>
{
    public string CustomerNo { get; set; }
    public BankAccountParams BankAccountParams { get; set; }
    public CashTransactionParams AccountTransactionsParams { get; set; }
}