using System.Collections.Immutable;

namespace OnlineBanking.Application.Models.CashTransaction.Responses;

public class CashTransactionListResponse
{
    public ImmutableList<CashTransactionResponse> CashTransactions { get; set; }

    public int Count { get; set; }

    public CashTransactionListResponse(ImmutableList<CashTransactionResponse> cashTransactions, int count)
    {
        CashTransactions = cashTransactions;
        Count = count;
    }
}