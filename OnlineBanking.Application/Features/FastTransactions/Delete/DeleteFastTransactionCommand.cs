namespace OnlineBanking.Application.Features.FastTransactions.Delete;

public class DeleteFastTransactionCommand : IRequest<ApiResult<Unit>>
{
    public Guid Id { get; set; }
    public string IBAN { get; set; }
}
