using System;

namespace OnlineBanking.Core.Domain.Exceptions;
public class AddressNotValidException : NotValidException
{
    internal AddressNotValidException() { }
    internal AddressNotValidException(string message) : base(message) { }
    internal AddressNotValidException(string message, Exception inner) : base(message, inner) { }
}