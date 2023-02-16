using System;
using MediatR;
using OnlineBanking.Application.Models;

namespace OnlineBanking.Application.Features.FastTransactions.Commands;

public class CreateFastTransactionCommand : IRequest<ApiResult<Unit>>
{
    public string RecipientIBAN { get; set; }
    public string RecipientName { get; set; }
    public decimal Amount { get; set; }
    public Guid BankAccountId { get; set; }
}
