namespace ASK.Filters.Operations;

public abstract record PropertyOperation(string Name, object? Value) : IOperation;

public record EqualOperation(string Name, object? Value) : PropertyOperation(Name, Value);

public record GreaterThanOperation : PropertyOperation
{
    public GreaterThanOperation(string Name, object? Value) : base(Name, Value)
    {
        if (Value is string)
            throw new FormatException("GreaterThan value cannot be a string");
    }
}

public record GreaterThanOrEqualOperation : PropertyOperation
{
    public GreaterThanOrEqualOperation(string Name, object? Value) : base(Name, Value)
    {
        if (Value is string)
            throw new FormatException("GreaterThanOrEqual value cannot be a string");
    }
}

public record LessThanOperation : PropertyOperation
{
    public LessThanOperation(string Name, object? Value) : base(Name, Value)
    {
        if (Value is string)
            throw new FormatException("GreaterThanOrEqual value cannot be a string");
    }
}

public record LessThanOrEqualOperation : PropertyOperation
{
    public LessThanOrEqualOperation(string Name, object? Value) : base(Name, Value)
    {
        if (Value is string)
            throw new FormatException("GreaterThanOrEqual value cannot be a string");
    }
}

public record ContainsOperation : PropertyOperation
{
    public ContainsOperation(string Name, object? Value) : base(Name, Value)
    {
        if (Value is not string)
            throw new FormatException("Contains value must be a string");
    }
}

public record StartWithOperation : PropertyOperation
{
    public StartWithOperation(string Name, object? Value) : base(Name, Value)
    {
        if (Value is not string)
            throw new FormatException("StartWith value must be a string");
    }
}

public record EndWithOperation : PropertyOperation
{
    public EndWithOperation(string Name, object? Value) : base(Name, Value)
    {
        if (Value is not string)
            throw new FormatException("EndWith value must be a string");
    }
}