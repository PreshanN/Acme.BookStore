using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Acme.BookStore.Employees;

public class CreateEmployeeDto : IValidatableObject
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public decimal SalaryAmount { get; set; }
    public string SalaryCurrency { get; set; } = "INR";
    public DateTime DateOfJoining { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DateOfJoining == default)
        {
            yield return new ValidationResult(
                "Date of joining is required.",
                new[] { nameof(DateOfJoining) });
        }
    }
}
