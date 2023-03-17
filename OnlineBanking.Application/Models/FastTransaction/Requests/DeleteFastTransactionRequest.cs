
namespace OnlineBanking.Application.Models.FastTransaction.Requests;

public class DeleteFastTransactionRequest
{
    public Guid Id { get; set; }
    public Guid BankAccountId { get; set; }
}
