using Acme.BookStore;
using Acme.BookStore.Departments;
using Acme.BookStore.Employees;
using EmployeeManagementSystem.Departments;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Xunit;

namespace Acme.BookStore.Departments;

public class DepartmentManager_Tests : BookStoreDomainTestBase<BookStoreDomainTestModule>
{
    private IDepartmentRepository _departmentRepository = null!;
    private IEmployeeRepository _employeeRepository = null!;
    private readonly DepartmentManager _departmentManager;

    public DepartmentManager_Tests()
        => _departmentManager = GetRequiredService<DepartmentManager>();

    protected override void AfterAddApplication(IServiceCollection services)
    {
        _departmentRepository = Substitute.For<IDepartmentRepository>();
        _employeeRepository = Substitute.For<IEmployeeRepository>();

        services.AddSingleton(_departmentRepository);
        services.AddSingleton(_employeeRepository);
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_When_Name_Exists()
    {
        // Arrange
        _departmentRepository.ExistsWithNameAsync("Engineering", Arg.Any<Guid?>())
            .Returns(true);

        // Act & Assert
        var ex = await Should.ThrowAsync<BusinessException>(
            () => _departmentManager.CreateAsync("Engineering", null));

        ex.Code.ShouldBe(EmployeeManagementSystemDomainErrorCodes.DuplicateDepartmentName);
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Department_When_Name_Is_Free()
    {
        // Act
        var department = await _departmentManager.CreateAsync("Sales", "Sells the products");

        // Assert
        department.Id.ShouldNotBe(Guid.Empty);
        department.Name.ShouldBe("Sales");
        await _departmentRepository.Received(1).ExistsWithNameAsync("Sales", Arg.Any<Guid?>());
    }
}
