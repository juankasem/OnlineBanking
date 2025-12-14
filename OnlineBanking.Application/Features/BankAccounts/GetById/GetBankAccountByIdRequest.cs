namespace OnlineBanking.Application.Features.BankAccounts.GetById;

public class GetBankAccountByIdRequest : IRequest<ApiResult<BankAccountResponse>>
{
    public Guid Id { get; set; }
}