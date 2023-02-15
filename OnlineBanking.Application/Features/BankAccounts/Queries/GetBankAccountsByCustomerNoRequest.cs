using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.BankAccount.Responses;
using OnlineBanking.Core.Helpers.Params;

namespace OnlineBanking.Application.Features.BankAccounts.Queries;
public class GetBankAccountsByCustomerNoRequest : IRequest<ApiResult<BankAccountListResponse>>
{
    public string CustomerNo { get; set; }

    public BankAccountParams BankAccountParams { get; set; }
}