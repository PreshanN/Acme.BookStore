using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Values;

namespace Acme.BookStore.Shared.ValueObjects
{
    public class Money : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }
        public Money(decimal amount, string currency)
        {
            if (amount < 0)
                throw new BusinessException("Money:Negative");
            Amount = amount;
            Currency = Check.NotNullOrWhiteSpace(currency, nameof(currency));
        }
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}
