using System.Linq.Expressions;
using ASK.Filters.OperationEvaluators;
using ASK.Filters.Operations;

namespace ASK.Filters;

public class FilterEvaluator<T>
{
    public static readonly FilterEvaluator<T> Default = new();

    private readonly Dictionary<Type, IBinaryOperationEvaluator> _binaryOperationEvaluators = new();
    private readonly Dictionary<Type, IUnaryOperationEvaluator> _unaryOperationEvaluators = new();

    public FilterEvaluator()
    {
        AddOperationEvaluator<NotOperation>(new NotOperationEvaluator());
        AddOperationEvaluator<AndOperation>(new AndOperationEvaluator());
        AddOperationEvaluator<OrOperation>(new OrOperationEvaluator());

        AddOperationEvaluator<EqualOperation>(new EqualOperationEvaluator());
        AddOperationEvaluator<GreaterThanOperation>(new GreaterThanOperationEvaluator());
        AddOperationEvaluator<GreaterThanOrEqualOperation>(new GreaterThanOrEqualOperationEvaluator());
        AddOperationEvaluator<LessThanOperation>(new LessThanOperationEvaluator());
        AddOperationEvaluator<LessThanOrEqualOperation>(new LessThanOrEqualOperationEvaluator());

        AddOperationEvaluator<ContainsOperation>(new ContainsOperationEvaluator());
        AddOperationEvaluator<StartWithOperation>(new StartWithOperationEvaluator());
        AddOperationEvaluator<EndWithOperation>(new EndWithOperationEvaluator());
    }

    public void AddOperationEvaluator<TOperation>(IBinaryOperationEvaluator binaryOperationEvaluator) where TOperation : IOperation
    {
        _binaryOperationEvaluators.Add(typeof(TOperation), binaryOperationEvaluator);
    }
    public void AddOperationEvaluator<TOperation>(IUnaryOperationEvaluator unaryOperationEvaluator) where TOperation : IOperation
    {
        _unaryOperationEvaluators.Add(typeof(TOperation), unaryOperationEvaluator);
    }

    public Expression<Func<T, bool>> Evaluate(Filter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        var parameter = Expression.Parameter(typeof(T), "x");
        var expression = EvaluateOperation(parameter, filter.Operation);
        return Expression.Lambda<Func<T, bool>>(expression, parameter);
    }

    private Expression EvaluateOperation(ParameterExpression parameter, IOperation operation)
    {
        return operation switch
        {
            BinaryOperation binaryOperation => EvaluateBinaryOperation(
                operation,
                EvaluateOperation(parameter, binaryOperation.LeftOperation),
                EvaluateOperation(parameter, binaryOperation.RightOperation)),
            UnaryOperation unaryOperation => EvaluateUnaryOperation(
                unaryOperation,
                EvaluateOperation(parameter, unaryOperation.Operation)),
            PropertyOperation propertyOperation => EvaluatePropertyOperation(parameter, propertyOperation),
            _ => throw new NotSupportedException($"Unsupported operation: {operation}")
        };
    }

    protected Expression EvaluateUnaryOperation(IOperation operation, Expression expression)
    {
        if(_unaryOperationEvaluators.TryGetValue(operation.GetType(), out var unary))
            return unary.Evaluate(expression);

        throw new Exception($"No evaluator found for operation {operation}");
    }

    protected Expression EvaluateBinaryOperation(IOperation operation, Expression left, Expression right)
    {
        if(_binaryOperationEvaluators.TryGetValue(operation.GetType(), out var binary))
            return binary.Evaluate(left,right);

        throw new Exception($"No evaluator found for operation {operation}");
    }

    protected virtual Expression EvaluatePropertyOperation(ParameterExpression parameter, PropertyOperation property)
    {
        return EvaluateBinaryOperation(property,
            Expression.Property(parameter, GetPropertyName(property)),
            Expression.Constant(property.Value));
    }

    protected virtual string GetPropertyName(PropertyOperation property)
    {
        return property.Name;
    }
}