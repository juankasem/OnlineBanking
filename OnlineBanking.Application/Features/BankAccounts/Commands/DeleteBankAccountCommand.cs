
namespace OnlineBanking.Application.Features.BankAccounts.Commands;

public class DeleteBankAccountCommand : IRequest<ApiResult<Unit>>
{
    public Guid BankAccountId { get; set; }
}