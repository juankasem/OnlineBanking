
namespace OnlineBanking.Application.Features.FastTransactions.Update;

public class UpdateFastTransactionCommand : IRequest<ApiResult<Unit>>
{
    public Guid Id { get; set; }
    public string IBAN { get; set; }
    public string RecipientIBAN { get; set; }
    public string RecipientName { get; set; }
    public decimal Amount { get; set; }
}
