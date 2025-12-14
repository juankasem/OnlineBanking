namespace OnlineBanking.Application.Features.BankAccounts.Activate;

public class ActivateBankAccountCommand : IRequest<ApiResult<Unit>>
{
    public Guid BankAccountId { get; set; }
}
