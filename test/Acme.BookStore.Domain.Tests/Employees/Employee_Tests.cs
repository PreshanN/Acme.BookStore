using System;
using Shouldly;
using Volo.Abp;
using Xunit;
using Acme.BookStore.Employees;
using Acme.BookStore.Shared.ValueObjects;

namespace Acme.BookStore.Employees;

public class Employee_Tests // Plain class - no ABP base runner, no DI, no DB
{
    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void SetSalary_Should_Throw_When_Amount_Is_Not_Positive(decimal amount)
    {
        // Arrange
        var employee = CreateEmployee();

        // Act
        var ex = Should.Throw<BusinessException>(() => employee.SetSalary(new Money(amount, "INR")));

        // Assert
        ex.Message.ShouldBe("Employee:InvalidSalary");
        employee.Salary.Amount.ShouldBe(850000m); // State remains unchanged
    }

    [Fact]
    public void New_Employee_Should_Start_On_Probation()
    {
        // Arrange & Act
        var employee = CreateEmployee(new DateTime(2026, 1, 5));

        // Assert
        employee.IsConfirmed.ShouldBeFalse();
        // Verifies your derived logic works with EmployeeConsts.ProbationMonths
        employee.ProbationEndDate.ShouldBe(new DateTime(2026, 7, 5));
    }

    [Fact]
    public void Confirm_Should_Throw_When_Probation_Is_Not_Completed()
    {
        // Arrange
        var employee = CreateEmployee(new DateTime(2026, 1, 5));

        // Act & Assert
        var ex = Should.Throw<BusinessException>(() => employee.Confirm(new DateTime(2026, 7, 4)));

        ex.Code.ShouldBe(EmployeeManagementSystemDomainErrorCodes.ProbationNotCompleted);
        employee.IsConfirmed.ShouldBeFalse();
    }

    [Fact]
    public void Confirm_Should_Set_Date_When_Probation_Is_Completed()
    {
        // Arrange
        var employee = CreateEmployee(new DateTime(2026, 1, 5));

        // Act
        employee.Confirm(new DateTime(2026, 7, 5));

        // Assert
        employee.IsConfirmed.ShouldBeTrue();
        employee.ConfirmationDate.ShouldBe(new DateTime(2026, 7, 5));
    }

    [Fact]
    public void Confirm_Should_Throw_When_Already_Confirmed()
    {
        // Arrange
        var employee = CreateEmployee(new DateTime(2026, 1, 5));
        employee.Confirm(new DateTime(2026, 7, 5));

        // Act & Assert
        var ex = Should.Throw<BusinessException>(() => employee.Confirm(new DateTime(2026, 8, 1)));

        ex.Code.ShouldBe(EmployeeManagementSystemDomainErrorCodes.EmployeeAlreadyConfirmed);
    }

    // Helper Factory Method utilizing your real primary constructor signatures
    private static Employee CreateEmployee(DateTime? dateOfJoining = null) =>
        new Employee(
            id: Guid.NewGuid(),
            employeeCode: "EMP-0001",
            fullName: "Priya Sharma",
            email: new EmailAddress("priya.sharma@example.com"),
            departmentId: Guid.NewGuid(),
            startingSalary: new Money(850000m, "INR"),
            dateOfJoining: dateOfJoining ?? new DateTime(2026, 1, 5)
        );
}
