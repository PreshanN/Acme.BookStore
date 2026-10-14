using FluentValidation;

namespace Acme.BookStore.Employees;

public class UpdateEmployeeSalaryDtoValidator : AbstractValidator<UpdateEmployeeSalaryDto>
{
    public UpdateEmployeeSalaryDtoValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().Length(EmployeeConsts.CurrencyLength);
    }
}
