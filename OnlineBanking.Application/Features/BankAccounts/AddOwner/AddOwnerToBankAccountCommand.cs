using OnlineBanking.Application.Models.Customer;

namespace OnlineBanking.Application.Features.BankAccounts.AddOwner;

public class AddOwnerToBankAccountCommand : IRequest<ApiResult<Unit>>
{
    public Guid BankAccountId { get; set; }
    public List<AccountOwnerDto> AccountOwners = [];
}