
namespace OnlineBanking.Application.Features.CashTransactions.Commands;

public class DeleteCashTransactionCommand : IRequest<ApiResult<Unit>>
{
    public Guid Id { get; set; }
}