using Acme.BookStore.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Acme.BookStore.Employees
{
    [Audited]
    public class Employee : FullAuditedAggregateRoot<Guid>, ISoftDelete, IMultiTenant
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

        /// <summary>
        /// Set once by <see cref="Confirm"/> when probation is completed. Null while on probation.
        /// </summary>
        public DateTime? ConfirmationDate { get; private set; }

        /// <summary>
        /// Derived, not persisted: the date on which probation ends.
        /// </summary>
        public DateTime ProbationEndDate => DateOfJoining.Date.AddMonths(EmployeeConsts.ProbationMonths);

        /// <summary>
        /// Derived, not persisted.
        /// </summary>
        public bool IsConfirmed => ConfirmationDate.HasValue;

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
            DateOfJoining = CheckDateOfJoining(dateOfJoining);
            SetSalary(startingSalary);
        }

        /// <summary>
        /// Confirms the employee at the end of probation.
        /// Throws if already confirmed or if <paramref name="confirmationDate"/> falls before
        /// <see cref="ProbationEndDate"/>. Only the date parts are compared.
        /// </summary>
        public void Confirm(DateTime confirmationDate)
        {
            if (IsConfirmed)
            {
                throw new BusinessException(EmployeeManagementSystemDomainErrorCodes.EmployeeAlreadyConfirmed)
                    .WithData("ConfirmationDate", ConfirmationDate.Value);
            }

            if (confirmationDate.Date < ProbationEndDate)
            {
                throw new BusinessException(EmployeeManagementSystemDomainErrorCodes.ProbationNotCompleted)
                    .WithData("ProbationEndDate", ProbationEndDate);
            }

            ConfirmationDate = confirmationDate;
        }

        private static DateTime CheckDateOfJoining(DateTime dateOfJoining)
        {
            if (dateOfJoining == default)
            {
                throw new BusinessException(EmployeeManagementSystemDomainErrorCodes.InvalidDateOfJoining);
            }

            return dateOfJoining;
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
