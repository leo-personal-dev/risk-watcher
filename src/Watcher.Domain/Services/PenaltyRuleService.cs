
using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Watcher.Domain.Services;

public class PenaltyRuleService : IPenaltyRuleService
{
    private readonly IPenaltyRuleRepository _repository;
    private static readonly ScriptOptions _scriptOptions = ScriptOptions.Default
        .AddReferences(typeof(Customer).Assembly)
        .AddImports("System", "System.Collections.Generic", "System.Linq");

    public PenaltyRuleService(IPenaltyRuleRepository repository)
    {
        _repository = repository;
    }

    public async Task<decimal> GetPenaltyMultiplierAsync(Cluster cluster, Customer customer)
    {
        var configurations = await _repository.GetAllAsync();
        foreach (var config in configurations)
        {
            if(await Evaluate(config.Trigger, customer))
            {
                return config.Effect;
            }
        }

        return 1m; // Default penalty multiplier
    }

    private static async Task<bool> Evaluate(string rule, Customer customer)
    {
        var script = CSharpScript.Create<bool>(
            rule,
            _scriptOptions,
            globalsType: typeof(ScriptContext)  // expose 'customer' as a global
        );

        script.Compile(); // pre-compile to catch syntax errors early

        var context = new ScriptContext { customer = customer };
        var result = await script.RunAsync(context);
        return result.ReturnValue;
    }
}