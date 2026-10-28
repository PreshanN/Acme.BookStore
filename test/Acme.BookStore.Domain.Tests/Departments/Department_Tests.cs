using Acme.BookStore;
using Acme.BookStore.Departments;
using Shouldly;
using System;
using Volo.Abp;
using Xunit;
using Acme.BookStore.Departments;
using Acme.BookStore.Shared.ValueObjects;
namespace EmployeeManagementSystem.Departments;

public class Department_Tests // plain class - no ABP base
{
    [Fact]
    public void AddTeam_Should_Throw_When_Name_Already_Exists()
    {
        // Arrange
        var department = new Department(Guid.NewGuid(), "Engineering");
        department.AddTeam(Guid.NewGuid(), "Backend");

        // Act
        var ex = Should.Throw<BusinessException>(
            () => department.AddTeam(Guid.NewGuid(), "Backend"));

        // Assert
        ex.Code.ShouldBe(EmployeeManagementSystemDomainErrorCodes.DuplicateTeamName);
        department.Teams.Count.ShouldBe(1);
    }

    [Fact]
    public void AddTeam_Should_Throw_When_Department_Already_Has_Ten_Teams()
    {
        // Arrange
        var department = new Department(Guid.NewGuid(), "Engineering");
        for (var i = 1; i <= 10; i++)
        {
            department.AddTeam(Guid.NewGuid(), $"Team {i}");
        }

        // Act
        var ex = Should.Throw<BusinessException>(
            () => department.AddTeam(Guid.NewGuid(), "Team 11"));

        // Assert
        ex.Code.ShouldBe(EmployeeManagementSystemDomainErrorCodes.TooManyTeams);
        department.Teams.Count.ShouldBe(10);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void SetName_Should_Throw_When_Name_Is_Empty(string name)
    {
        // Arrange
        var department = new Department(Guid.NewGuid(), "Engineering");

        // Act & Assert
        Should.Throw<ArgumentException>(() => department.SetName(name));
        department.Name.ShouldBe("Engineering");
    }
}
 