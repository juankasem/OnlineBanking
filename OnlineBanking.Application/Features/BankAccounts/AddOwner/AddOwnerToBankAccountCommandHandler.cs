using OnlineBanking.Application.Models.Customer;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;

namespace OnlineBanking.Application.Features.BankAccounts.AddOwner;

/// <summary>
/// Handles requests to add owners to a bank account.
/// Validates account and customers exist, then associates them.
/// </summary>
public class AddOwnerToBankAccountCommandHandler(
    IUnitOfWork uow, 
    ILogger<AddOwnerToBankAccountCommandHandler> logger) : 
    IRequestHandler<AddOwnerToBankAccountCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<AddOwnerToBankAccountCommandHandler> _logger = logger;

    /// <summary>
    /// Handles the request to add owners to a bank account.
    /// </summary>
    public async Task<ApiResult<Unit>> Handle(
        AddOwnerToBankAccountCommand request, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.AccountOwners);

        var result = new ApiResult<Unit>();
        var bankAccountId = request.BankAccountId;

        _logger.LogInformation(
            "Processing add owner request for bank account: {BankAccountId}",
            bankAccountId);

        // Validate bank account exists
        var bankAccount = await _uow.BankAccounts.GetByIdAsync(bankAccountId);
        if (bankAccount is null)
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, "Id", bankAccountId));
            return result;
        }

        var accountOwners = request.AccountOwners;
        if (accountOwners.Count == 0)
        {
            _logger.LogWarning("No customers provided for account owner assignment");
            result.AddError(ErrorCode.BadRequest, "At least one customer must be provided");
            return result;
        }

        var customers = await RetrieveAndValidateCustomersAsync(accountOwners, result, cancellationToken);
        if (customers is null || customers.Count == 0)
        {
            return result;
        }

        // Associate customers with bank account
        AssociateOwnersWithBankAccount(bankAccount, customers);

        // Persist changes
        _uow.BankAccounts.Update(bankAccount);
        await _uow.SaveAsync();

        _logger.LogInformation(
            "Successfully added {OwnerCount} owner(s) to bank account: {BankAccountId}",
             customers.Count,
             bankAccountId);

        return result;
    }

    /// <summary>
    /// Retrieves and validates that the bank account exists.
    /// </summary>
    private async Task<IReadOnlyList<Customer>?> RetrieveAndValidateCustomersAsync(
       List<AccountOwnerDto> accountOwners,
        ApiResult<Unit> result,
        CancellationToken cancellationToken)
    {
        var customers = new List<Customer>();

        foreach (var owner in accountOwners)
        {
            var customer = await _uow.Customers.GetByCustomerNoAsync(owner.CustomerNo);
            if (customer is null)
            {
                result.AddError(ErrorCode.NotFound,
                string.Format(CustomerErrorMessages.NotFound, "Customer No.", customer.CustomerNo));

                return null;
            }
            customers.Add(customer);
        }

        return customers;
    }

    /// <summary>
    /// Associates customers as owners of the bank account.
    /// </summary>
    private static void AssociateOwnersWithBankAccount(
        BankAccount bankAccount,
        IReadOnlyList<Customer> customers)
    {
        ArgumentNullException.ThrowIfNull(bankAccount);
        ArgumentNullException.ThrowIfNull(customers);

        if (customers.Count == 0)
        {
            return;
        }

        try
        {
            foreach (var customer in customers)
            {
                var bankAccountOwner = CustomerBankAccount.Create(bankAccount.Id, customer.Id);
                bankAccount.AddOwnerToBankAccount(bankAccountOwner);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Error associating customers with bank account {bankAccount.Id}",
                ex);
        }
    }
}
