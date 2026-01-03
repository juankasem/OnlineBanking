
namespace OnlineBanking.Application.Features.BankAccounts;

public class BankAccountErrorMessages
{
    public const string NoIdentifierFound = "No identifier {0} found in route or query";
    public const string NotFound = "No bank account found with {0} {1}";
    public const string AlreadyExists = "Bank account No. {0} already exists";
    public const string DeleteNotPossible = "Only the owner of a post can delete it";
    public const string CreateNotAuthorized = "Unauthorized operation";
    public const string UnauthorizedOperation = "Unauthorized operation. As the user {0} is not an owner of the bank account";
    public const string Inactive = "Account with IBAN {0} is not active.";
    public const string Unknown = "Unknown error.";
}
