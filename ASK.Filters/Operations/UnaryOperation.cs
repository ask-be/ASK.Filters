using System.Linq.Expressions;

namespace ASK.Filters.Operations;

internal abstract record UnaryOperation(IOperation Operation) : IOperation
{
    public abstract Expression GetExpression(Expression expression);
}

internal record NotOperation(IOperation Operation) : UnaryOperation(Operation)
{
    public override Expression GetExpression(Expression expression)
    {
        return Expression.Not(expression);
    }
}