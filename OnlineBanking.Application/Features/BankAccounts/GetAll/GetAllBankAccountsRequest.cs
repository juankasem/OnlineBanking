namespace OnlineBanking.Application.Features.BankAccounts.GetAll;

public class GetAllBankAccountsRequest : IRequest<ApiResult<PagedList<BankAccountDto>>>
{
    public BankAccountParams BankAccountParams { get; set; }
}