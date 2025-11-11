
namespace OnlineBanking.Application.Features.CashTransactions;

public class CashTransactionErrorMessages
{
    public const string NotFound = "No cash transaction found with ID {0}";
    public const string DeleteNotPossible = "Only the owner of bank account can delete it";
    public const string UnAuthorizedOperation = "Unauthorized operation. As the user {0} is not an owner of the bank account that initiated transaction";
    public const string InsufficientFunds = "Sorry! You don't have enough funds to complete transaction";
    public const string UnknownError = "Sorry! Unable to complete transaction";
}
