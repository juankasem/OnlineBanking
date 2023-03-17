namespace OnlineBanking.Application.Features.CreditCards.Messages;

public class CreditCardsErrorMessages
{
    public const string NotFound = "No credit card found with {0} {1}";
    public const string DeleteNotPossible = "Only the owner of a post can delete it";
    public const string CreateNotAuthorized = "Unauthorized operation";
}
