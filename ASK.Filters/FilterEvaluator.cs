using System.Linq.Expressions;
using ASK.Filters.Operations;

namespace ASK.Filters;

public class FilterEvaluator
{
    public static readonly FilterEvaluator Default = new();

    public Expression<Func<T, bool>> GetExpression<T>(FilterPropertyType filterPropertyType)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var expression = GetOperationExpression<T>(parameter,filterPropertyType.Operation);
        return Expression.Lambda<Func<T, bool>>(expression, parameter);
    }

    private Expression GetOperationExpression<T>(ParameterExpression parameter, IOperation operation)
    {
        return operation switch
        {
            BinaryOperation binaryOperation => binaryOperation.GetExpression(
                GetOperationExpression<T>(parameter, binaryOperation.LeftOperation),
                GetOperationExpression<T>(parameter, binaryOperation.RightOperation)),
            UnaryOperation unaryOperation => unaryOperation.GetExpression(
                GetOperationExpression<T>(parameter, unaryOperation.Operation)),
            PropertyOperation propertyOperation => GetPropertyExpression<T>(parameter, propertyOperation),
            _ => throw new NotSupportedException($"Unsupported operation: {operation}")
        };
    }

    protected virtual Expression GetPropertyExpression<T>(ParameterExpression parameter, PropertyOperation property)
    {
        return property.GetExpression(
            Expression.Property(parameter, GetPropertyName(property)),
            Expression.Constant(property.Value));
    }

    protected virtual string GetPropertyName(PropertyOperation property)
    {
        return property.Name;
    }
}