using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.BankAccount.Responses;

namespace OnlineBanking.Application.Features.BankAccounts.Queries;

public class GetBankAccountByAccountNoRequest : IRequest<ApiResult<BankAccountResponse>>
{
    public string AccountNo { get; set; }
}