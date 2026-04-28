using System.Globalization;
using System.Linq;
using Watcher.Domain.Entities;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Watcher.Domain.Services;

public static class ClusterRuleEvaluator
{

    private static readonly ScriptOptions _scriptOptions = ScriptOptions.Default
        .AddReferences(typeof(Customer).Assembly)
        .AddImports("System", "System.Collections.Generic", "System.Linq");

    public static async Task<bool> Evaluate(string rule, Customer customer)
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
