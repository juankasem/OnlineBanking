
namespace OnlineBanking.Application.Features.BankAccounts.Queries;

public class GetBankAccountByIdRequest : IRequest<ApiResult<BankAccountResponse>>
{
    public Guid Id { get; set; }
}