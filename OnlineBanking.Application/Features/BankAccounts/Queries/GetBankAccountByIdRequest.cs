using System;
using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.BankAccount.Responses;

namespace OnlineBanking.Application.Features.BankAccounts.Queries;

public class GetBankAccountByIdRequest : IRequest<ApiResult<BankAccountResponse>>
{
    public Guid Id { get; set; }
}