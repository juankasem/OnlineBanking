
namespace OnlineBanking.Application.Features.CashTransactions.Create;

public class CreateCashTransactionCommand : IRequest<ApiResult<Unit>>
{
    public string ReferenceNo { get; set; }
    public CashTransactionType CashTransactionType { get; set; }
    public BankAssetType InitiatedBy { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }
    public decimal Fees { get; set; }
    public string Description { get; set; }
    public PaymentType PaymentType { get; set; }
    public DateTime TransactionDate { get; set; }
    public CashTransactionStatus Status { get; set; }
}
