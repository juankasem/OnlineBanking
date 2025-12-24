
namespace OnlineBanking.Application.Features.Customers.GetBankAccounts;

public class GetCustomerBankAccountsRequest : IRequest<ApiResult<List<BankAccountDto>>>
{
    public string CustomerNo { get; set; }
}