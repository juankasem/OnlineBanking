
namespace OnlineBanking.Application.Features.CashTransactions;

public class CashTransactionErrorMessages
{
    public const string NotFound = "No cash transaction found with ID {0}";
    public const string DeleteNotPossible = "Only the owner of bank account can delete it";
    public const string UnauthorizedOperation = "Unauthorized operation. As the user {0} is not an owner of the bank account that initiated the transaction";
    public const string InsufficientFunds = "Sorry! You don't have enough funds to complete transaction";
    public const string IBANMismatch = "IBAN mismatch between route and request body.";
    public const string UnsupportedTransactionType = "Unsupported transaction type: {0}";
    public const string UnknownError = "An unexpected error occurred while processing the {0}. Please contact support if the problem persists.";
}
