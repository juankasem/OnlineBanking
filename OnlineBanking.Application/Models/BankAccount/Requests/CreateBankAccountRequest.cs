
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Application.Models.BankAccount.Requests;

public class CreateBankAccountRequest 
{
    public string AccountNo { get; set; }
    public string IBAN { get;  set; }
    public BankAccountType Type { get;  set; }
    public int BranchId { get;  set; }
    public decimal Balance { get;  set; }
    public decimal AllowedBalanceToUse { get;  set; }
    public decimal MinimumAllowedBalance { get;  set; }
    public decimal Debt { get;  set; }
    public int CurrencyId { get;  set; }
    public List<string> CustomerNos { get;  set; }
}