
namespace OnlineBanking.Application.Features.BankAccounts.Commands;

public class ActivateBankAccountCommand : IRequest<ApiResult<Unit>>
{
    public Guid BankAccountId { get; set; }
}
