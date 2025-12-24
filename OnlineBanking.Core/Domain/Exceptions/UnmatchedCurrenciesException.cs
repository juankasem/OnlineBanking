
namespace OnlineBanking.Core.Domain.Exceptions;

public class UnmatchedCurrenciesException(string message) : DomainException(message);
