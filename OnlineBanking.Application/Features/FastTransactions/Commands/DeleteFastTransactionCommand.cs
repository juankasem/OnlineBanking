namespace OnlineBanking.Application.Features.FastTransactions.Commands;

public class DeleteFastTransactionCommand : IRequest<ApiResult<Unit>>
{
    public Guid Id { get; set; }
    public string IBAN { get; set; }
}
