using OnlineBanking.Application.Models.CashTransaction.Base;

namespace OnlineBanking.Application.Models.CashTransaction.Requests;

public class UpdateCashTransactionRequest
{
    public string Id { get; set; }
    public BaseCashTransactionDto CashTransaction { get; set; }
}
