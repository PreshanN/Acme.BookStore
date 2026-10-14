using Acme.BookStore.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Acme.BookStore.Employees
{
    public class Employee : AggregateRoot<Guid>, ISoftDelete, IMultiTenant
    {
        private readonly List<EmployeeQualification> _qualifications = new();
        public IReadOnlyList<EmployeeQualification> Qualifications => _qualifications.AsReadOnly();

        // The ONLY way to add a qualification — external code cannot
        // new-up an EmployeeQualification and push it in directly.
        public void AddQualification(Guid id, string degree, int yearCompleted)
        {
            if (_qualifications.Any(q => q.Degree == degree))
                throw new BusinessException("Employee:DuplicateQualification");
            _qualifications.Add(new EmployeeQualification(id, Id, degree, yearCompleted));
        }


        public Guid? TenantId { get; private set; } // owner of this row

        public string EmployeeCode { get; private set; }
        public string FullName { get; private set; }
        public EmailAddress Email { get; private set; }
        public Address? CurrentAddress { get; private set; } // optional until ChangeAddress is called
        public Money Salary { get; private set; }
        public Guid DepartmentId { get; private set; }
        public DateTime DateOfJoining { get; private set; }
        public bool IsDeleted { get; set; } // ISoftDelete
                                            // Required by the ORM — never call directly.
        protected Employee() { }
        // Primary constructor — guarantees a valid Employee is always born.
        public Employee(
        Guid id,
        string employeeCode,
        string fullName,
        EmailAddress email,
        Guid departmentId,
        Money startingSalary,
        DateTime dateOfJoining,
        Guid? tenantId = null)
        : base(id)
        {
            TenantId = tenantId;
            EmployeeCode = Check.NotNullOrWhiteSpace(employeeCode, nameof(employeeCode));
            FullName = Check.NotNullOrWhiteSpace(fullName, nameof(fullName));
            Email = Check.NotNull(email, nameof(email));
            DepartmentId = departmentId;
            DateOfJoining = dateOfJoining;
            SetSalary(startingSalary);
        }
        public void SetSalary(Money salary)
        {
            if (salary.Amount <= 0)
                throw new BusinessException("Employee:InvalidSalary");
            Salary = salary;
        }
        public void ChangeAddress(Address newAddress)
        {
            CurrentAddress = Check.NotNull(newAddress, nameof(newAddress));
        }
        public void AssignToDepartment(Guid departmentId)
        {
            DepartmentId = departmentId;
        }
    }

    
}
