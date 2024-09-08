using System.Linq.Expressions;
using System.Reflection;
using ASK.Filters.OperationTokens;

namespace ASK.Filters;

internal static class FilterExpressionBuilder
{
    private static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains",BindingFlags.Instance | BindingFlags.Public , [typeof(string)])!;
    private static readonly MethodInfo StartsWithMethod = typeof(string).GetMethod("StartsWith", BindingFlags.Instance | BindingFlags.Public, [typeof(string)])!;
    private static readonly MethodInfo EndsWithMethod = typeof(string).GetMethod("EndsWith", BindingFlags.Instance | BindingFlags.Public, [typeof(string)])!;
    
    public static Expression<Func<T, bool>> BuildExpression<T>(Filter filter)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var expression = CreateExpression<T>(parameter, filter.OperationToken);
        return Expression.Lambda<Func<T, bool>>(expression, parameter);
    }

    private static Expression CreateExpression<T>(ParameterExpression parameter, OperationToken operationToken)
    {
        switch (operationToken)
        {
            case BinaryOperationToken binary:
                // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
                return operationToken.Operator switch
                {
                    FilterOperator.And => Expression.AndAlso(CreateExpression<T>(parameter, binary.LeftOperation), CreateExpression<T>(parameter, binary.RightOperation)),
                    FilterOperator.Or => Expression.Or(CreateExpression<T>(parameter, binary.LeftOperation), CreateExpression<T>(parameter, binary.RightOperation)),
                    _ => throw new InvalidOperationException($"Unknown binary operand: {operationToken.Operator}")
                };
            case UnaryOperationToken unary:
                // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
                return operationToken.Operator switch
                {
                    FilterOperator.Not => Expression.Not(CreateExpression<T>(parameter,unary.Operation)),
                    _ => throw new InvalidOperationException($"Unknown unary operand: {operationToken.Operator}")
                };
            case PropertyOperationToken property:
                var propertyName = Expression.Property(parameter, property.Name);
                var constant = Expression.Constant(property.Value);

                // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
                return operationToken.Operator switch
                {
                    FilterOperator.Equal => Expression.Equal(propertyName, constant),
                    FilterOperator.GreaterThan => Expression.GreaterThan(propertyName, constant),
                    FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(propertyName, constant),
                    FilterOperator.LessThan => Expression.LessThan(propertyName, constant),
                    FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(propertyName, constant),
                    FilterOperator.Contains => Expression.Call(propertyName, ContainsMethod, constant),
                    FilterOperator.StartWith => Expression.Call(propertyName, StartsWithMethod, constant),
                    FilterOperator.EndWith => Expression.Not(Expression.Call(propertyName, EndsWithMethod, constant)),
                    _ => throw new InvalidOperationException($"Unknown property operand: {operationToken.Operator}")
                };
            default:
                throw new InvalidOperationException($"Unknown operation: {operationToken.Operator}");
        }
    }

}