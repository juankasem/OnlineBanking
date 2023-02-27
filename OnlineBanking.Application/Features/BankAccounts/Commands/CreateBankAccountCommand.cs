using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.BankAccount;
using OnlineBanking.Application.Models.Customer;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Application.Features.BankAccounts.Commands;

public class CreateBankAccountCommand : IRequest<ApiResult<Unit>>
{
    public string AccountNo { get; private set; }
    public string IBAN { get; private set; }
    public BankAccountType Type { get; private set; }
    public int CurrencyId { get; set; }
    public int BranchId { get; private set; }
    public AccountBalanceDto AccountBalance { get; set; }
    public List<AccountOwnerDto> AccountOwners = new List<AccountOwnerDto>();
}
