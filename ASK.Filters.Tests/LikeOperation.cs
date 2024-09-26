using System.Linq.Expressions;
using System.Reflection;
using ASK.Filters.OperationEvaluators;
using ASK.Filters.Operations;
using Microsoft.EntityFrameworkCore;

namespace ASK.Filters.Tests;

public class LikeOperationEvaluator : IBinaryOperationEvaluator
{
    private static readonly MethodInfo LikeMethod = typeof(DbFunctionsExtensions)
        .GetMethod(nameof(DbFunctionsExtensions.Like), [typeof(DbFunctions), typeof(string), typeof(string)])!;
    private static readonly Expression EfFunctions = Expression.Constant(EF.Functions);

    public Expression Evaluate(Expression left, Expression right) =>  Expression.Call(LikeMethod, EfFunctions, left, right);
}

public record LikeOperation : PropertyOperation
{
    public LikeOperation(string Name, object? Value) : base(Name, Value)
    {
        if(Value is not string)
            throw new FormatException("Like value must be a string");
    }
}