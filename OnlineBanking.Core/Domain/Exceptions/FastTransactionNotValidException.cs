using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBanking.Core.Domain.Exceptions
{
    public class FastTransactionNotValidException : NotValidException
    {
        internal FastTransactionNotValidException() { }
        internal FastTransactionNotValidException(string message) : base(message) { }
        internal FastTransactionNotValidException(string message, Exception inner) : base(message, inner) { }
    }
}