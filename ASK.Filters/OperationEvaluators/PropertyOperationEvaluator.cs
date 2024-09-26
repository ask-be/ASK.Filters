using System.Linq.Expressions;
using System.Reflection;

namespace ASK.Filters.OperationEvaluators;

public class EqualOperationEvaluator : IBinaryOperationEvaluator
{
    public Expression Evaluate(Expression left, Expression right) => Expression.Equal(left, right);
}

public class GreaterThanOperationEvaluator : IBinaryOperationEvaluator
{
    public Expression Evaluate(Expression left, Expression right) => Expression.GreaterThan(left, right);
}

public class GreaterThanOrEqualOperationEvaluator : IBinaryOperationEvaluator
{
    public Expression Evaluate(Expression left, Expression right) => Expression.GreaterThanOrEqual(left, right);
}

public class LessThanOperationEvaluator : IBinaryOperationEvaluator
{
    public Expression Evaluate(Expression left, Expression right) => Expression.LessThan(left, right);
}

public class LessThanOrEqualOperationEvaluator : IBinaryOperationEvaluator
{
    public Expression Evaluate(Expression left, Expression right) => Expression.LessThanOrEqual(left, right);
}

public class ContainsOperationEvaluator : IBinaryOperationEvaluator
{
    private static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains", BindingFlags.Instance | BindingFlags.Public, [typeof(string)])!;

    public Expression Evaluate(Expression left, Expression right) => Expression.Call(left, ContainsMethod, right);
}
public class StartWithOperationEvaluator : IBinaryOperationEvaluator
{
    private static readonly MethodInfo StartWithMethod = typeof(string).GetMethod("StartWith", BindingFlags.Instance | BindingFlags.Public, [typeof(string)])!;

    public Expression Evaluate(Expression left, Expression right) => Expression.Call(left, StartWithMethod, right);
}
public class EndWithOperationEvaluator : IBinaryOperationEvaluator
{
    private static readonly MethodInfo EndWithMethod = typeof(string).GetMethod("EndWith", BindingFlags.Instance | BindingFlags.Public, [typeof(string)])!;

    public Expression Evaluate(Expression left, Expression right) => Expression.Call(left, EndWithMethod, right);
}
