using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Values;

namespace Acme.BookStore.Departments
{
    public class ContactInfo : ValueObject
    {
        public string? Email { get; }
        public string? Phone { get; }
        public ContactInfo(string? email, string? phone) { Email = email; Phone = phone; }
        protected override IEnumerable<object> GetAtomicValues() { yield return Email; yield return Phone; }
    }
}
