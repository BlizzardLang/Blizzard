using System;
using System.Collections.Generic;
using System.Linq;

namespace Blizzard;

/// <summary>
/// A blizzard function
/// </summary>
internal struct Function
{
    /// <summary>
    /// The name of the function
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// The method the function performs
    /// </summary>
    private Func<object[], object> Method { get; set; }

    public Function(string name, Func<object[], object> method)
    {
        Name = name;
        Method = method;
    }

    /// <summary>
    /// Executes the method of the function with the given parameters
    /// </summary>
    /// <param name="parameters">The parameters to pass to the method</param>
    /// <returns>The result of the method</returns>
    public object Exec(params object[] parameters)
        => Method(parameters);
}

/// <summary>
/// Manages the builtin functions for blizzard
/// </summary>
internal static class BuiltinFunctions
{
    /// <returns><see cref="builtinFunctions"/> as a dictionary mapping functions' names to themselves</returns>
    internal static Dictionary<string, Function> GetDict()
        => builtinFunctions.ToDictionary(f => f.Name);

    /// <summary>
    /// The builtin functions for blizzard
    /// </summary>
    private static Function[] builtinFunctions => new Function[]
    {
        new Function("WRITE", (object[] @params) =>
        {
            Console.Write(string.Join(" ", @params));
            return new object();
        }),
        new Function("WRITELN", (object[] @params) =>
        {
            Console.WriteLine(string.Join(" ", @params));
            return new object();
        })
    };
}
