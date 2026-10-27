using Acme.BookStore.Departments;
using Acme.BookStore.Employees;
using Acme.BookStore.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Acme.BookStore
{
    public class EmployeeTestDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Department, Guid> _departmentRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeTestDataSeedContributor(
            IRepository<Department, Guid> departmentRepository,
            IEmployeeRepository employeeRepository)
        {
            _departmentRepository = departmentRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            await _departmentRepository.InsertAsync(new Department(TestData.EngineeringId, "Engineering"));
            await _departmentRepository.InsertAsync(new Department(TestData.FinanceId, "Finance"));

            await _employeeRepository.InsertAsync(NewEmployee(TestData.PriyaId, "EMP-0001", "Priya Sharma"));
            await _employeeRepository.InsertAsync(NewEmployee(TestData.ArjunId, "EMP-0002", "Arjun Mehta"));
            await _employeeRepository.InsertAsync(NewEmployee(TestData.MeeraId, "EMP-0003", "Meera Iyer"));

            // one employee owned by a tenant - for the isolation test 
            await _employeeRepository.InsertAsync(NewEmployee(TestData.TechCorpEmployeeId, "EMP-0001", "Kavya Rao", TestData.TechCorpTenantId));
        }

        private static Employee NewEmployee(Guid id, string code, string name, Guid? tenantId = null) =>
            new Employee(
                id,
                code,
                name,
                new EmailAddress(code.ToLowerInvariant() + "@example.com"),
                TestData.EngineeringId,
                new Money(850000m, "INR"),
                new DateTime(2026, 1, 5),
                tenantId
            );
    }
}