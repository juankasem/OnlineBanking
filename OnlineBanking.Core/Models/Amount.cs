using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBanking.Core.Models
{
    public class Amount
    {
        public decimal Value { get; }

        public Amount(decimal value) =>
            Value = value < 0 ? 0 : value;

        public Amount Add(Amount amount) => new (Value + amount);
        public Amount Subtract(Amount amount) => new (Value - amount);

        public static implicit operator decimal(Amount? amount) => amount?.Value ?? 0;
    }
}