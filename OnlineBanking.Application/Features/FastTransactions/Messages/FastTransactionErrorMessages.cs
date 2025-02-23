namespace OnlineBanking.Application.Features.FastTransactions.Messages;

public class FastTransactionErrorMessages
{
    public const string NotFound = "No fasttransaction found with ID {0}";
    public const string DeleteNotPossible = "Only the owner of bank account can delete it";
    public const string UnAuthorizedOperation = " Unauthorized operation. As the user {0} is not an owner of the bank account that initiated transaction";
    public const string UnknownError = "Sorry! Unable to create fast transaction";
}