using System.Linq.Expressions;

namespace ASK.Filters.OperationEvaluators;

public interface IBinaryOperationEvaluator
{
    Expression Evaluate(Expression left, Expression right);
}

public class AndOperationEvaluator : IBinaryOperationEvaluator
{
    public Expression Evaluate(Expression left, Expression right) => Expression.AndAlso(left, right);
}
public class OrOperationEvaluator : IBinaryOperationEvaluator
{
    public Expression Evaluate(Expression left, Expression right) => Expression.Or(left, right);
}