namespace OnlineBanking.Core.Domain.Exceptions
{
    public class FastTransactionNotValidException : NotValidException
    {
        internal FastTransactionNotValidException() { }
        internal FastTransactionNotValidException(string message) : base(message) { }
        internal FastTransactionNotValidException(string message, Exception inner) : base(message, inner) { }
    }
}