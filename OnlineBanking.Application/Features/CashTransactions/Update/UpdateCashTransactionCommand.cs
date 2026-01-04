using OnlineBanking.Application.Models.CashTransaction.Base;

namespace OnlineBanking.Application.Features.CashTransactions.Update;

public class UpdateCashTransactionCommand : IRequest<ApiResult<Unit>>
{
    public Guid Id { get; set; }
    public BaseCashTransaction CashTransaction { get; set; }
}