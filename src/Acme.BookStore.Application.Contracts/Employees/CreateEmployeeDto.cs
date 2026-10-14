using System;

namespace Acme.BookStore.Employees;

public class CreateEmployeeDto
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public decimal SalaryAmount { get; set; }
    public string SalaryCurrency { get; set; } = "INR";
    public DateTime DateOfJoining { get; set; }
}
