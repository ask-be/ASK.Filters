using System.Linq.Expressions;

namespace ASK.Filters.Operations;

public abstract record UnaryOperation(IOperation Operation) : IOperation
{
    public abstract UnaryExpression GetExpression(Expression expression);
}

public record NotOperation(IOperation Operation) : UnaryOperation(Operation)
{
    public override UnaryExpression GetExpression(Expression expression)
    {
        return Expression.Not(expression);
    }
}