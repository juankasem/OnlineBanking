using System;
using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CashTransaction.Base;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Application.Features.CashTransactions.Commands;

public class UpdateCashTransactionCommand : IRequest<ApiResult<Unit>>
{
    public Guid Id { get; set; }
    public BaseCashTransactionDto BaseCasTransaction { get; set; }
}