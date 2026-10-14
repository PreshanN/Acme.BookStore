using Acme.BookStore.Departments;
using Acme.BookStore.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace Acme.BookStore.Employees
{
    public class EmployeeManager : DomainService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRepository<Department, Guid> _departmentRepository;

        public EmployeeManager(
            IEmployeeRepository employeeRepository,
            IRepository<Department, Guid> departmentRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<Employee> CreateAsync(
            string employeeCode, string fullName, EmailAddress email,
            Guid departmentId, Money startingSalary, DateTime dateOfJoining)
        {
            await CheckEmployeeCodeIsUniqueAsync(employeeCode);
            await CheckDepartmentExistsAsync(departmentId);

            return new Employee(
                GuidGenerator.Create(), employeeCode, fullName, email,
                departmentId, startingSalary, dateOfJoining, CurrentTenant.Id);
        }

        public async Task TransferAsync(Employee employee, Guid newDepartmentId)
        {
            await CheckDepartmentExistsAsync(newDepartmentId);
            employee.AssignToDepartment(newDepartmentId);
        }

        private async Task CheckEmployeeCodeIsUniqueAsync(string employeeCode)
        {
            if (await _employeeRepository.FindByEmployeeCodeAsync(employeeCode) != null)
            {
                throw new BusinessException(
                    EmployeeManagementSystemDomainErrorCodes.EmployeeCodeAlreadyExists)
                    .WithData("EmployeeCode", employeeCode);
            }
        }

        private async Task CheckDepartmentExistsAsync(Guid departmentId)
        {
            if (await _departmentRepository.FindAsync(departmentId) == null)
            {
                throw new BusinessException(
                EmployeeManagementSystemDomainErrorCodes.DepartmentNotFound)
                .WithData("DepartmentId", departmentId);
            }
        }
    }
}
