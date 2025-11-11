using OnlineBanking.Application.Models.BankAccount;
using OnlineBanking.Application.Models.BankAccount.Responses;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;

namespace OnlineBanking.Application.Mappings.BankAccounts;

public interface IBankAccountMapper
{
    BankAccountDto MapToDtoModel(BankAccount bankAccount);
    BankAccountResponse MapToResponseModel(BankAccount bankAccount,
                                           IReadOnlyList<Customer> bankAccountOwners,
                                           IReadOnlyList<CashTransaction> cashTransactions);
}