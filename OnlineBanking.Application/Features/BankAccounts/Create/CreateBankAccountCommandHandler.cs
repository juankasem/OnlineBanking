using OnlineBanking.Application.Features.Customers;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;

namespace OnlineBanking.Application.Features.BankAccounts.Create;

public class CreateBankAccountCommandHandler : IRequestHandler<CreateBankAccountCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow;
    public CreateBankAccountCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ApiResult<Unit>> Handle(CreateBankAccountCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();

        if (await _uow.BankAccounts.ExistsAsync(request.AccountNo))
        {
            result.AddError(ErrorCode.CustomerAlreadyExists,
            string.Format(BankAccountErrorMessages.AlreadyExists, request.AccountNo));

            return result;
        }

        var bankAccount = CreateBankAccount(request);

        if (request.CustomerNos.Count > 0)
        {
            var accountOwners = new List<Customer>();

            foreach (var customerNo in request.CustomerNos)
            {
                var customer = await _uow.Customers.GetByCustomerNoAsync(customerNo);
                if (customer is null)
                {
                    result.AddError(ErrorCode.NotFound,
                    string.Format(CustomerErrorMessages.NotFound, "Id", customer.Id));

                    return result;
                }
                accountOwners.Add(customer);

                var bankAccountOwner = CustomerBankAccount.Create(bankAccount.Id, customer.Id);
                bankAccount.AddOwnerToBankAccount(bankAccountOwner);
            }
        }

        await _uow.BankAccounts.AddAsync(bankAccount);
        await _uow.SaveAsync();

        return result;
    }

    #region Private helper methods
    private static Core.Domain.Aggregates.BankAccountAggregate.BankAccount CreateBankAccount(CreateBankAccountCommand request) =>
                Core.Domain.Aggregates.BankAccountAggregate.
                BankAccount.Create(request.AccountNo, request.IBAN, request.Type,
                                    request.BranchId, request.Balance,
                                    request.AllowedBalanceToUse, request.MinimumAllowedBalance,
                                    request.Debt, request.CurrencyId);
    #endregion
}
