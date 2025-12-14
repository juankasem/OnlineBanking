
namespace OnlineBanking.Application.Features.CashTransactions.Delete;

public class DeleteCashTransactionCommand : IRequest<ApiResult<Unit>>
{
    public Guid Id { get; set; }
}