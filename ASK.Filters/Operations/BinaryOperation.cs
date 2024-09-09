using System.Linq.Expressions;

namespace ASK.Filters.Operations;

public abstract record BinaryOperation(IOperation LeftOperation, IOperation RightOperation) : IOperation
{
    public abstract Expression GetExpression(Expression leftOperation, Expression rightOperation);
}

internal record AndOperation(IOperation LeftOperation, IOperation RightOperation) : BinaryOperation(LeftOperation, RightOperation)
{
    public override Expression GetExpression(Expression leftOperation, Expression rightOperation)
    {
        return Expression.AndAlso(leftOperation,rightOperation);
    }
}

internal record OrOperation(IOperation LeftOperation, IOperation RightOperation) : BinaryOperation(LeftOperation, RightOperation)
{
    public override Expression GetExpression(Expression leftOperation, Expression rightOperation)
    {
        return Expression.Or(leftOperation,rightOperation);
    }
}