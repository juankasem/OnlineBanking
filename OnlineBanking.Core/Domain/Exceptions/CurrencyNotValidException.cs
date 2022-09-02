using System;

namespace OnlineBanking.Core.Domain.Exceptions;
public class CurrencyNotValidException : NotValidException
{
    internal CurrencyNotValidException() { }
    internal CurrencyNotValidException(string message) : base(message) { }
    internal CurrencyNotValidException(string message, Exception inner) : base(message, inner) { }
}