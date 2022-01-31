namespace Blizzard;

/// <summary>
/// The type of variable
/// </summary>
internal enum VariableType
{
    STR, INT, DEC
}

/// <summary>
/// A blizzard variable
/// </summary>
internal struct Variable
{
    /// <summary>
    /// The name of the variable
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The type of the variable
    /// </summary>
    public VariableType Type { get; set; }
    /// <summary>
    /// The variable's value
    /// </summary>
    public object Value { get; set; }

    public Variable(string name, VariableType type, object value)
    {
        Name = name;
        Type = type;
        Value = value;
    }

    /// <summary>
    /// Gets the correct variable type given the string encoded type
    /// </summary>
    /// <param name="type">The string encoded variable type</param>
    /// <returns>The corresponding <see cref="VariableType"/> for <paramref name="type"/></returns>
    /// <exception cref="NotImplementedException">Thrown when <paramref name="type"/> is not valid</exception>
    public static VariableType VariableTypeFrom(string type)
    {
        return type switch {
            "str" => VariableType.STR,
            "int" => VariableType.INT,
            "dec" => VariableType.DEC,

            _ => throw new NotImplementedException($"Variable type {type} not implemented.")
        };
    }
}
