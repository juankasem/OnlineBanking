namespace OnlineBanking.Application.Features.FastTransactions.Commands;

public class CreateFastTransactionCommand : IRequest<ApiResult<Unit>>
{
    public string IBAN { get; set; }
    public string RecipientIBAN { get; set; }
    public string RecipientName { get; set; }
    public decimal Amount { get; set; }
}
