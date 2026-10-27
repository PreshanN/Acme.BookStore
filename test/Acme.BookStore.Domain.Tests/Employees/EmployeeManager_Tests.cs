using Acme.BookStore;
using Acme.BookStore.Departments;
using Acme.BookStore.Employees;
using Acme.BookStore.Shared.ValueObjects;

using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Acme.BookStore.Employees; // Aligned to your real project namespace

public class EmployeeManager_Tests : BookStoreDomainTestBase<BookStoreDomainTestModule>
{
    private IRepository<Employee, Guid> _employeeRepository = null!; // Uses standard repository interface
    private IRepository<Department, Guid> _departmentRepository = null!;

    private readonly EmployeeManager _employeeManager;

    public EmployeeManager_Tests() =>
        _employeeManager = GetRequiredService<EmployeeManager>();

    protected override void AfterAddApplication(IServiceCollection services)
    {
        _employeeRepository = Substitute.For<IRepository<Employee, Guid>>();
        _departmentRepository = Substitute.For<IRepository<Department, Guid>>();

        services.AddSingleton(_employeeRepository); // Fakes replace real registrations
        services.AddSingleton(_departmentRepository);
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_When_Code_Exists()
    {
        // Arrange
        _employeeRepository.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Employee, bool>>>())
            .Returns(Task.FromResult<Employee?>(NewEmployee("EMP-0001")));

        // Act
        var ex = await Should.ThrowAsync<BusinessException>(() => CreateAsync("EMP-0001"));

        // Assert
        ex.Code.ShouldBe("Employee:DuplicateCode"); // Aligned to your error string
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Employee_When_Rules_Pass()
    {
        // Arrange: Configure your code lookup fake to return null (meaning code is available)
        _employeeRepository.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Employee, bool>>>())
            .Returns(Task.FromResult<Employee?>(null));

        // Act
        var employee = await CreateAsync("EMP-0099");

        // Assert
        employee.ShouldNotBeNull();
        employee.EmployeeCode.ShouldBe("EMP-0099");
        employee.DepartmentId.ShouldBe(DepartmentId);
    }

    private static readonly Guid DepartmentId = Guid.Parse("5f8a1c2e-0d3b-4c9a-9e61-1a2b3c4d5e99");

    private Task<Employee> CreateAsync(string code) =>
        _employeeManager.CreateAsync(
            employeeCode: code,
            fullName: "Rahul Verma",
            email: new EmailAddress("rahul.verma@example.com"),
            departmentId: DepartmentId,
            startingSalary: new Money(900000m, "INR"),
            dateOfJoining: new DateTime(2026, 9, 1)
        );


    private static Employee NewEmployee(string code) =>
        new Employee(
            Guid.NewGuid(),
            code,
            "Someone",
            new EmailAddress("someone@example.com"),
            DepartmentId,
            new Money(1m, "INR"),
            new DateTime(2026, 1, 5)
        );
}
