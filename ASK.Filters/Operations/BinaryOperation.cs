using System.Linq.Expressions;

namespace ASK.Filters.Operations;

public abstract record BinaryOperation(IOperation LeftOperation, IOperation RightOperation) : IOperation
{
    public abstract BinaryExpression GetExpression(Expression leftOperation, Expression rightOperation);
}

public record AndOperation(IOperation LeftOperation, IOperation RightOperation) : BinaryOperation(LeftOperation, RightOperation)
{
    public override BinaryExpression GetExpression(Expression leftOperation, Expression rightOperation)
    {
        return Expression.AndAlso(leftOperation,rightOperation);
    }
}

public record OrOperation(IOperation LeftOperation, IOperation RightOperation) : BinaryOperation(LeftOperation, RightOperation)
{
    public override BinaryExpression GetExpression(Expression leftOperation, Expression rightOperation)
    {
        return Expression.Or(leftOperation,rightOperation);
    }
}