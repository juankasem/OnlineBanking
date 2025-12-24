
namespace OnlineBanking.Core.Domain.Exceptions;

public class InsufficientFundsException(string message) : DomainException(message)
{
}