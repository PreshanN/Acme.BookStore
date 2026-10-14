using System;
using FluentValidation;

namespace Acme.BookStore.Employees;

public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeDtoValidator()
    {
        RuleFor(x => x.EmployeeCode)
            .NotEmpty()
            .MaximumLength(EmployeeConsts.MaxEmployeeCodeLength)
            .Matches(@"^EMP-\d{4}$").WithMessage("Employee code must look like EMP-0001.");

        RuleFor(x => x.FullName).NotEmpty().MaximumLength(EmployeeConsts.MaxFullNameLength);

        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(EmployeeConsts.MaxEmailLength);

        RuleFor(x => x.DepartmentId).NotEmpty();

        RuleFor(x => x.SalaryAmount).GreaterThan(0);

        RuleFor(x => x.SalaryCurrency).NotEmpty().Length(EmployeeConsts.CurrencyLength);

        RuleFor(x => x.DateOfJoining)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.Today.AddDays(90))
            .WithMessage("Date of joining cannot be more than 90 days in the future.");
    }
}
