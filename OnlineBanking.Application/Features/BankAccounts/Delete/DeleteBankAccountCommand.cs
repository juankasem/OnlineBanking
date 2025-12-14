namespace OnlineBanking.Application.Features.BankAccounts.Delete;

public class DeleteBankAccountCommand : IRequest<ApiResult<Unit>>
{
    public Guid BankAccountId { get; set; }
}