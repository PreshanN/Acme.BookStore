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
            // Engineering department with code and teams
            var engineering = new Department(
                TestData.EngineeringId,
                "Engineering",
                "Builds the products"
            );

            engineering.SetCode("ENG");
            engineering.AddTeam(TestData.BackendTeamId, "Backend");
            engineering.AddTeam(TestData.FrontendTeamId, "Frontend");

            await _departmentRepository.InsertAsync(engineering);

            // Finance department
            var finance = new Department(TestData.FinanceId, "Finance");
            finance.SetCode("FIN");

            await _departmentRepository.InsertAsync(finance);

            // Legal department - no employees, so it can be deleted in tests
            var legal = new Department(TestData.LegalId, "Legal");
            legal.SetCode("LEG");

            await _departmentRepository.InsertAsync(legal);

            // Engineering department belonging to another tenant
            var techCorpEngineering = new Department(
                TestData.TechCorpDepartmentId,
                "Engineering",
                tenantId: TestData.TechCorpTenantId
            );

            techCorpEngineering.SetCode("ENG");

            await _departmentRepository.InsertAsync(techCorpEngineering);

            // Seed employees - keep the existing code
            await _employeeRepository.InsertAsync(
                NewEmployee(TestData.PriyaId, "EMP-0001", "Priya Sharma")
            );

            await _employeeRepository.InsertAsync(
                NewEmployee(TestData.ArjunId, "EMP-0002", "Arjun Mehta")
            );

            await _employeeRepository.InsertAsync(
                NewEmployee(TestData.MeeraId, "EMP-0003", "Meera Iyer")
            );

            // Employee owned by another tenant for the isolation test
            await _employeeRepository.InsertAsync(
                NewEmployee(
                    TestData.TechCorpEmployeeId,
                    "EMP-0001",
                    "Kavya Rao",
                    TestData.TechCorpTenantId
                )
            );
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