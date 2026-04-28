using FluentValidation;
using Watcher.Domain.Commands.Request;

namespace Watcher.Domain.Validators;

public class CreateJobCategoryCommandValidator : AbstractValidator<CreateJobCategoryCommand>
{
    public CreateJobCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(3, 50).WithMessage("Name must be between 3 and 50 characters");

        RuleFor(x => x.Multiplier)
            .GreaterThan(0).WithMessage("Multiplier must be greater than 0");

        RuleFor(x => x.Keywords)
            .NotEmpty().WithMessage("At least one keyword is required")
            .Must(k => k.Count <= 50).WithMessage("Maximum 50 keywords allowed");

        RuleForEach(x => x.Keywords)
            .NotEmpty().WithMessage("Keywords cannot be empty");
    }
}