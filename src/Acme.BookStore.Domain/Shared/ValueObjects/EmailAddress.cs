using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Values;

namespace Acme.BookStore.Shared.ValueObjects
{
    public class EmailAddress : ValueObject
    {
        public string Value { get; }
        public EmailAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
                throw new BusinessException("EmailAddress:Invalid");
            Value = value;
        }
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
