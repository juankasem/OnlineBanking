namespace OnlineBanking.Core.Domain.Exceptions;

public class BranchAddressNotValidException : NotValidException
{
    internal BranchAddressNotValidException() { }
    internal BranchAddressNotValidException(string message) : base(message) { }
    internal BranchAddressNotValidException(string message, Exception inner) : base(message, inner) { }
}