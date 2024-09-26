using System.Linq.Expressions;

namespace ASK.Filters.OperationEvaluators;

public interface IUnaryOperationEvaluator
{
    Expression Evaluate(Expression expression);
}

public class NotOperationEvaluator : IUnaryOperationEvaluator
{
    public Expression Evaluate(Expression expression) => Expression.Not(expression);
}