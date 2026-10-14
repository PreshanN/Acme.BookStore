using Acme.BookStore.Shared.Departments;
using FluentValidation;

namespace Acme.BookStore.Departments;

public class AddDepartmentTeamDtoValidator : AbstractValidator<AddDepartmentTeamDto>
{
    public AddDepartmentTeamDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(DepartmentConsts.MaxTeamNameLength);
    }
}
