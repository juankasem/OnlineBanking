using System.Collections.Generic;
using MediatR;
using OnlineBanking.Application.Models.Customer.Base;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Application.Models.BankAccount.Requests;

public class CreateBankAccountRequest : IRequest<ApiResult<Unit>>
{
    public string AccountNo { get; private set; }
    public string IBAN { get; private set; }
    public BankAccountType Type { get; private set; }
    public int BranchId { get; private set; }
    public decimal Balance { get; private set; }
    public decimal AllowedBalanceToUse { get; private set; }
    public decimal MinimumAllowedBalance { get; private set; }
    public decimal Debt { get; private set; }
    public int CurrencyId { get; private set; }
    public bool IsActive { get; private set; }
    public IEnumerable<BaseCustomerDto> Owners { get; private set; }
}
