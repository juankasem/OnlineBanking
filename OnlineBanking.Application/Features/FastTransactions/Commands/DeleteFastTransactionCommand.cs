using MediatR;
using OnlineBanking.Application.Models;

namespace OnlineBanking.Application.Features.FastTransactions.Commands;

public class DeleteFastTransactionCommand : IRequest<ApiResult<uint>>
{
    public Guid Id { get; set; }
}
