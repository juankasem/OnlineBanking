using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBanking.Core.Domain.Exceptions
{
    public class BankAccountNotValidException : NotValidException
    {
        internal BankAccountNotValidException() { }
        internal BankAccountNotValidException(string message) : base(message) { }
        internal BankAccountNotValidException(string message, Exception inner) : base(message, inner) { }
    }
}