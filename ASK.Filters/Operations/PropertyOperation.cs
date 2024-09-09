using System.Linq.Expressions;
using System.Reflection;

namespace ASK.Filters.Operations;

public abstract record PropertyOperation(string Name, object Value) : IOperation
{
    public abstract Expression GetExpression(Expression left, Expression right);
}

internal record EqualOperation(string Name, object Value) : PropertyOperation(Name, Value)
{
    public override Expression GetExpression(Expression left, Expression right)
    {
        return Expression.Equal(left, right);
    }
}

internal record GreaterThanOperation : PropertyOperation
{
    public GreaterThanOperation(string Name, object Value) : base(Name, Value)
    {
        if(Value is string)
            throw new FormatException("GreaterThan value cannot be a string");
    }

    public override Expression GetExpression(Expression left, Expression right)
    {
        return Expression.GreaterThan(left, right);
    }
}

internal record GreaterThanOrEqualOperation : PropertyOperation
{
    public GreaterThanOrEqualOperation(string Name, object Value) : base(Name, Value)
    {
        if(Value is string)
            throw new FormatException("GreaterThanOrEqual value cannot be a string");
    }

    public override Expression GetExpression(Expression left, Expression right)
    {
        return Expression.GreaterThanOrEqual(left, right);
    }
}

internal record LessThanOperation : PropertyOperation
{
    public LessThanOperation(string Name, object Value) : base(Name, Value)
    {
        if(Value is string)
            throw new FormatException("GreaterThanOrEqual value cannot be a string");
    }

    public override Expression GetExpression(Expression left, Expression right)
    {
        return Expression.LessThan(left, right);
    }
}

internal record LessThanOrEqualOperation : PropertyOperation
{
    public LessThanOrEqualOperation(string Name, object Value) : base(Name, Value)
    {
        if(Value is string)
            throw new FormatException("GreaterThanOrEqual value cannot be a string");
    }

    public override Expression GetExpression(Expression left, Expression right)
    {
        return Expression.LessThanOrEqual(left, right);
    }
}

internal record ContainsOperation : PropertyOperation
{
    private static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains",BindingFlags.Instance | BindingFlags.Public , [typeof(string)])!;

    public ContainsOperation(string Name, object Value) : base(Name, Value)
    {
        if(Value is not string)
            throw new FormatException("Contains value must be a string");
    }

    public override Expression GetExpression(Expression left, Expression right)
    {
        return Expression.Call(left, ContainsMethod, right);
    }
}

internal record StartWithOperation : PropertyOperation
{
    private static readonly MethodInfo StartWithMethod = typeof(string).GetMethod("StartWith",BindingFlags.Instance | BindingFlags.Public , [typeof(string)])!;

    public StartWithOperation(string Name, object Value) : base(Name, Value)
    {
        if(Value is not string)
            throw new FormatException("StartWith value must be a string");
    }

    public override Expression GetExpression(Expression left, Expression right)
    {
        return Expression.Call(left, StartWithMethod, right);
    }
}

internal record EndWithOperation : PropertyOperation
{
    private static readonly MethodInfo EndWithMethod = typeof(string).GetMethod("EndWith",BindingFlags.Instance | BindingFlags.Public , [typeof(string)])!;

    public EndWithOperation(string Name, object Value) : base(Name, Value)
    {
        if(Value is not string)
            throw new FormatException("EndWith value must be a string");
    }

    public override Expression GetExpression(Expression left, Expression right)
    {
        return Expression.Call(left, EndWithMethod, right);
    }
}