using System;
using MediatR;
using OnlineBanking.Application.Models;

namespace OnlineBanking.Application.Features.CashTransactions.Commands;

public class DeleteCashTransactionCommand : IRequest<ApiResult<Unit>>
{
    public Guid Id { get; set; }
}