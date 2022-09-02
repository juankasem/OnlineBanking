using System;
using System.Collections.Generic;
using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.Customer;

namespace OnlineBanking.Application.Features.BankAccounts.Commands;
public class AddOwnerToBankAccountCommand : IRequest<ApiResult<Unit>>
{
    public Guid BankAccountId { get; set; }
    public List<AccountOwnerDto> AccountOwners = new List<AccountOwnerDto>();
}