using System;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.Employees;

public class EmployeeDto : EntityDto<Guid>
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;           // from EmailAddress.Value
    public decimal? SalaryAmount { get; set; }                  // from Salary.Amount (null if hidden)
    public string? SalaryCurrency { get; set; }                 // from Salary.Currency
    public Guid DepartmentId { get; set; }
    public DateTime DateOfJoining { get; set; }
    public AddressDto? CurrentAddress { get; set; }
}
