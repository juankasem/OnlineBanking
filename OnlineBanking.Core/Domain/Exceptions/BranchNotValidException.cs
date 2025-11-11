namespace OnlineBanking.Core.Domain.Exceptions;

public class BranchNotValidException : NotValidException
{
    internal BranchNotValidException() { }
    internal BranchNotValidException(string message) : base(message) { }
    internal BranchNotValidException(string message, Exception inner) : base(message, inner) { }
}
