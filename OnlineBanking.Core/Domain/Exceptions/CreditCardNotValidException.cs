using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBanking.Core.Domain.Exceptions
{
    public class CreditCardNotValidException : NotValidException
    {
         internal CreditCardNotValidException() { }
        internal CreditCardNotValidException(string message) : base(message) { }
        internal CreditCardNotValidException(string message, Exception inner) : base(message, inner) { }
    }
}