using System;
using MediatR;
using OnlineBanking.Application.Models;

namespace OnlineBanking.Application.Features.BankAccounts.Commands;
public class ActivateBankAccountCommand : IRequest<ApiResult<Unit>>
{
    public Guid BankAccountId { get; set; }
}
