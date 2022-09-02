
namespace OnlineBanking.Application.Features.BankAccounts;
public class BankAccountErrorMessages
{
    public const string NotFound = "No bank account found with {0} {1}";
    public const string DeleteNotPossible = "Only the owner of a post can delete it";
    public const string CreateNotAuthorized = "Unauthorized operation";
}
