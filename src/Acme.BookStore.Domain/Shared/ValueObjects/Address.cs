using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Values;

namespace Acme.BookStore.Shared.ValueObjects
{
    public class Address : ValueObject
    {
        public string Line1 { get; }
        public string City { get; }
        public string State { get; }
        public string PostalCode { get; }
        public Address(string line1, string city, string state, string postalCode)
        {
            Line1 = Check.NotNullOrWhiteSpace(line1, nameof(line1));
            City = Check.NotNullOrWhiteSpace(city, nameof(city));
            State = Check.NotNullOrWhiteSpace(state, nameof(state));
            PostalCode = Check.NotNullOrWhiteSpace(postalCode, nameof(postalCode));
        }
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Line1;
            yield return City;
            yield return State;
            yield return PostalCode;
        }
    }
}
