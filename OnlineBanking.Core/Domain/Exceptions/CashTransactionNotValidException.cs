namespace OnlineBanking.Core.Domain.Exceptions
{
    public class CashTransactionNotValidException : NotValidException
    {
        internal CashTransactionNotValidException() { }
        internal CashTransactionNotValidException(string message) : base(message) { }
        internal CashTransactionNotValidException(string message, Exception inner) : base(message, inner) { }
    }
}