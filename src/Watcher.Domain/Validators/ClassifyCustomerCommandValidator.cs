using FluentValidation;
using Watcher.Domain.Commands.Request;

namespace Watcher.Domain.Validators;

public class ClassifyCustomerCommandValidator : AbstractValidator<ClassifyCustomerCommand>
{
    public ClassifyCustomerCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required");

        RuleFor(x => x.Score)
            .InclusiveBetween(0, 1000).WithMessage("Score must be between 0 and 1000");

        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(18).WithMessage("Age must be at least 18");

        RuleFor(x => x.JobTitle)
            .NotEmpty().WithMessage("JobTitle is required")
            .MaximumLength(100).WithMessage("JobTitle must not exceed 100 characters");
    }
}