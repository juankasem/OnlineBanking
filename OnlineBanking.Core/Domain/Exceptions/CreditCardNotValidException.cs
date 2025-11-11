namespace OnlineBanking.Core.Domain.Exceptions
{
    public class CreditCardNotValidException : NotValidException
    {
        internal CreditCardNotValidException() { }
        internal CreditCardNotValidException(string message) : base(message) { }
        internal CreditCardNotValidException(string message, Exception inner) : base(message, inner) { }
    }
}