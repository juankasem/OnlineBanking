using OnlineBanking.Application.Models.BankAccount;

namespace OnlineBanking.Application.Features.Customers.Queries;

public class GetCustomerBankAccountsRequest : IRequest<ApiResult<List<BankAccountDto>>>
{
    public Guid CustomerId { get; set; }
}