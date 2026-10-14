using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Acme.BookStore.Employees
{
    public class EmployeeQualification : Entity<Guid>
    {
        public Guid EmployeeId { get; private set; }
        public string Degree { get; private set; }
        public int YearCompleted { get; private set; }
        protected EmployeeQualification() { }
        internal EmployeeQualification(
        Guid id, Guid employeeId, string degree, int yearCompleted)
        : base(id)
        {
            EmployeeId = employeeId;
            Degree = Check.NotNullOrWhiteSpace(degree, nameof(degree));
            YearCompleted = yearCompleted;
        }
    }
}
