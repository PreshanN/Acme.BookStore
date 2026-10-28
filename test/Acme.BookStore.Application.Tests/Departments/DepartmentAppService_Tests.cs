using System;
using System.Reflection; // Required for GetCustomAttribute
using System.Threading.Tasks;
using Acme.BookStore;
using Acme.BookStore.Departments;
using Acme.BookStore.Permissions; // Added to locate your BookStorePermissions
using Microsoft.AspNetCore.Authorization; // Required for AuthorizeAttribute
using Shouldly;
using Volo.Abp;
using Volo.Abp.Modularity;
using Xunit;

namespace acme.BookStore.Departments;

public abstract class DepartmentAppService_Tests<TStartupModule> : BookStoreApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IDepartmentAppService _departmentAppService;
    private readonly IDepartmentRepository _departmentRepository;

    protected DepartmentAppService_Tests()
    {
        _departmentAppService = GetRequiredService<IDepartmentAppService>();
        _departmentRepository = GetRequiredService<IDepartmentRepository>();
    }

    [Fact]
    public async Task CreateAsync_Should_Save_And_Return_Dto()
    {
        // Act
        var result = await _departmentAppService.CreateAsync(new CreateDepartmentDto
        {
            Name = "Quality Assurance",
            Code = "qa-01",
            ContactEmail = "qa@example.com"
        });

        // Assert
        result.Id.ShouldNotBe(Guid.Empty);
        result.Code.ShouldBe("QA-01"); // normalised by the domain
        result.ContactInfoEmail.ShouldBe("qa@example.com"); // flattened mapping
        result.IsActive.ShouldBeTrue();

        await WithUnitOfWorkAsync(async () =>
        {
            var saved = await _departmentRepository.GetAsync(result.Id);
            saved.Name.ShouldBe("Quality Assurance");
        });
    }

    [Fact]
    public async Task CreateWithTeamsAsync_Should_Save_Nothing_When_A_Rule_Fails()
    {
        // Arrange
        var input = new CreateDepartmentWithTeamsDto { Name = "Mobile" };
        for (var i = 1; i <= 11; i++)
        {
            input.TeamNames.Add($"Team {i}");
        }

        // Act & Assert
        var ex = await Should.ThrowAsync<BusinessException>(
            () => _departmentAppService.CreateWithTeamsAsync(input));

        ex.Code.ShouldBe(BookStoreDomainErrorCodes.TooManyTeams);

        await WithUnitOfWorkAsync(async () =>
        {
            (await _departmentRepository.GetListAsync())
                .ShouldNotContain(d => d.Name == "Mobile"); // Unit of Work rolled back
        });
    }

    [Fact]
    public void DepartmentAppService_Should_Require_The_Default_Permission()
    {
        // Act
        var attribute = typeof(DepartmentAppService).GetCustomAttribute<AuthorizeAttribute>();

        // Assert
        attribute.ShouldNotBeNull();
        attribute.Policy.ShouldBe(BookStorePermissions.Departments.Default);
    }

    [Theory]
    [InlineData(nameof(DepartmentAppService.CreateAsync), BookStorePermissions.Departments.Create)]
    [InlineData(nameof(DepartmentAppService.UpdateAsync), BookStorePermissions.Departments.Edit)]
    [InlineData(nameof(DepartmentAppService.DeleteAsync), BookStorePermissions.Departments.Delete)]
    public void Write_Methods_Should_Require_Their_Permission(string methodName, string policy)
    {
        // Act
        var attribute = typeof(DepartmentAppService).GetMethod(methodName)!
            .GetCustomAttribute<AuthorizeAttribute>();

        // Assert
        attribute.ShouldNotBeNull();
        attribute.Policy.ShouldBe(policy);
    }
}
