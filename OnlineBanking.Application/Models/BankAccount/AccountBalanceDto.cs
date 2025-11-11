
namespace OnlineBanking.Application.Models.BankAccount;

public class AccountBalanceDto
{
    public decimal Balance { get; private set; }
    public decimal AllowedBalanceToUse { get; private set; }
    public decimal MinimumAllowedBalance { get; private set; }
    public decimal Debt { get; private set; }

    public AccountBalanceDto()
    {
    }

    public AccountBalanceDto(decimal balance, decimal allowedBalanceToUse,
                            decimal minimumAllowedBalance, decimal debt)
    {
        Balance = balance;
        AllowedBalanceToUse = allowedBalanceToUse;
        MinimumAllowedBalance = minimumAllowedBalance;
        Debt = debt;
    }
}