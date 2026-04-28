using FluentValidation;
using Watcher.Domain.Entities;

namespace Watcher.Api.Validators;

public class ClusterConfigurationRequestValidator : AbstractValidator<ClusterConfiguration>
{
    public ClusterConfigurationRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.ScoreMin).InclusiveBetween(300, 1000);
        RuleFor(x => x.ScoreMax).InclusiveBetween(300, 1000);
        RuleFor(x => x.ScoreMax).GreaterThanOrEqualTo(x => x.ScoreMin);
        RuleFor(x => x.AgeMin).InclusiveBetween(18, 100);
        RuleFor(x => x.AgeMax).InclusiveBetween(18, 100);
        RuleFor(x => x.AgeMax).GreaterThanOrEqualTo(x => x.AgeMin);
        RuleFor(x => x.BaseLimit).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Cap).GreaterThanOrEqualTo(x => x.BaseLimit);
        RuleFor(x => x.DebtRule).NotEmpty();
    }
}