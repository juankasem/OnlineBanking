
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate.Events;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;

namespace OnlineBanking.Application.Features.BankAccounts.Create;

public class CreateBankAccountCommandHandler : IRequestHandler<CreateBankAccountCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<MakeDepositCommandHandler> _logger;

    public CreateBankAccountCommandHandler(IUnitOfWork uow, ILogger<MakeDepositCommandHandler> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<ApiResult<Unit>> Handle(CreateBankAccountCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting creating a new {type} bank account", request.Type.ToString());

        var result = new ApiResult<Unit>();

        if (await _uow.BankAccounts.ExistsAsync(request.AccountNo))
        {
            result.AddError(ErrorCode.CustomerAlreadyExists,
            string.Format(BankAccountErrorMessages.AlreadyExists, request.AccountNo));
            return result;
        }

        var bankAccount = CreateBankAccount(request);

        if (request.CustomerNos.Length > 0)
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


        // Persist changes
        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            _logger.LogInformation("Bank account of Id {bankAccountId} of account No: {accountNo} & " +
                                    "IBAN: {iban}, type:{type}, balance: {balance} is created",
                                   bankAccount.Id,
                                   bankAccount.AccountNo,
                                   bankAccount.IBAN,
                                   bankAccount.Type,
                                   bankAccount.Balance);
        }
        else
        {
            result.AddError(ErrorCode.UnknownError, BankAccountErrorMessages.Unknown);
            _logger.LogError($"Creating bank account failed!");
        }

        return result;
    }

    #region Private helper methods
    private static BankAccount CreateBankAccount(CreateBankAccountCommand request) =>
                BankAccount.Create(request.AccountNo, 
                                    request.IBAN, 
                                    request.Type,
                                    request.BranchId, 
                                    request.Balance,
                                    request.AllowedBalanceToUse, 
                                    request.MinimumAllowedBalance,
                                    request.Debt, 
                                    request.CurrencyId);
    #endregion
}
