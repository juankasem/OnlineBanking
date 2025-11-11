
namespace OnlineBanking.Application.Features.BankAccounts.Queries;

public class GetAllBankAccountsRequest : IRequest<ApiResult<PagedList<BankAccountDto>>>
{
    public BankAccountParams BankAccountParams { get; set; }
}