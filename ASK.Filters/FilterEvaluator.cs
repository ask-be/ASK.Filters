using System.Linq.Expressions;
using ASK.Filters.Operations;

namespace ASK.Filters;

public class FilterEvaluator<T>
{
    public static readonly FilterEvaluator<T> Default = new();

    public Expression<Func<T, bool>> GetExpression(Filter filter)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var expression = GetOperationExpression(parameter,filter.Operation);
        return Expression.Lambda<Func<T, bool>>(expression, parameter);
    }

    private Expression GetOperationExpression(ParameterExpression parameter, IOperation operation)
    {
        return operation switch
        {
            BinaryOperation binaryOperation => binaryOperation.GetExpression(
                GetOperationExpression(parameter, binaryOperation.LeftOperation),
                GetOperationExpression(parameter, binaryOperation.RightOperation)),
            UnaryOperation unaryOperation => unaryOperation.GetExpression(
                GetOperationExpression(parameter, unaryOperation.Operation)),
            PropertyOperation propertyOperation => GetPropertyExpression(parameter, propertyOperation),
            _ => throw new NotSupportedException($"Unsupported operation: {operation}")
        };
    }

    protected virtual Expression GetPropertyExpression(ParameterExpression parameter, PropertyOperation property)
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