using Acme.BookStore.Shared.Departments;
using FluentValidation;

namespace Acme.BookStore.Departments;

public class CreateDepartmentDtoValidator : AbstractValidator<CreateDepartmentDto>
{
    public CreateDepartmentDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(DepartmentConsts.MaxNameLength);

        RuleFor(x => x.Description)
            .MaximumLength(DepartmentConsts.MaxDescriptionLength);

        RuleFor(x => x.ContactEmail)
            .EmailAddress().MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));

        RuleFor(x => x.ContactPhone)
            .Matches(@"^\+?[0-9\- ]{7,20}$")
            .WithMessage("Phone must contain 7-20 digits, spaces or dashes.")
            .When(x => !string.IsNullOrWhiteSpace(x.ContactPhone));
    }
}
