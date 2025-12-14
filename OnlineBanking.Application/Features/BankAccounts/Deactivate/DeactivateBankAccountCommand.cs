namespace OnlineBanking.Application.Features.BankAccounts.Deactivate;

public class DeactivateBankAccountCommand : IRequest<ApiResult<Unit>>
{
    public Guid BankAccountId { get; set; }
}