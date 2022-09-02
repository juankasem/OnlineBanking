using System;
using System.Collections.Generic;
using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.BankAccount;

namespace OnlineBanking.Application.Features.Customers.Queries;

public class GetCustomerBankAccountsRequest : IRequest<ApiResult<List<BankAccountDto>>>
{
    public Guid CustomerId { get; set; }
}