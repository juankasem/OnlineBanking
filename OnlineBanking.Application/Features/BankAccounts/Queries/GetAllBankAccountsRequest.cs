using MediatR;
using OnlineBanking.Application.Helpers;
using OnlineBanking.Application.Helpers.Params;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.BankAccount;

namespace OnlineBanking.Application.Features.BankAccounts.Queries;

public class GetAllBankAccountsRequest : IRequest<ApiResult<PagedList<BankAccountDto>>>
{
    public BankAccountParams BankAccountParams { get; set; }
}