namespace OnlineBanking.Application.Features.BankAccounts.Create;

public class CreateBankAccountCommand : IRequest<ApiResult<Unit>>
{
    public string AccountNo { get; private set; }
    public string IBAN { get; private set; }
    public BankAccountType Type { get; private set; }
    public int CurrencyId { get; set; }
    public int BranchId { get; private set; }
    public decimal Balance { get; set; }
    public decimal Debt { get; set; }
    public decimal AllowedBalanceToUse { get; set; }
    public decimal MinimumAllowedBalance { get; set; }
    public List<string> CustomerNos { get; set; }
}