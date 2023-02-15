using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.BankAccount;
using OnlineBanking.Core.Helpers;
using OnlineBanking.Core.Helpers.Params;

namespace OnlineBanking.Application.Features.BankAccounts.Queries;

public class GetAllBankAccountsRequest : IRequest<ApiResult<PagedList<BankAccountDto>>>
{
    public BankAccountParams BankAccountParams { get; set; }
}