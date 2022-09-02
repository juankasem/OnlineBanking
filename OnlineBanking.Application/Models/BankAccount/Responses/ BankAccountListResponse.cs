using System.Collections.Immutable;

namespace OnlineBanking.Application.Models.BankAccount.Responses;

public class BankAccountListResponse
{
    public ImmutableList<BankAccountResponse> BankAccounts { get; set; }
    public int Count { get; set; }

    public  BankAccountListResponse(ImmutableList<BankAccountResponse> bankAccounts, int count)
    {
        BankAccounts = bankAccounts;
        Count = count;
    }
}