using FluentValidation;
using Watcher.Domain.Commands.Request;

namespace Watcher.Domain.Validators;

public class GetJobCategoryByIdCommandValidator : AbstractValidator<GetJobCategoryByIdCommand>
{
    public GetJobCategoryByIdCommandValidator()
    {
        RuleFor(x => x.JobCategoryId)
            .NotEmpty().WithMessage("JobCategoryId is required");
    }
}