using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.BankAccount.Responses;

namespace OnlineBanking.Application.Features.BankAccounts.Queries;
public class GetAllBankAccountsRequest : IRequest<ApiResult<BankAccountListResponse>>
{
}