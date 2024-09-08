namespace ASK.Filters.OperationTokens;

internal record BinaryOperationToken(FilterOperator Operator, OperationToken LeftOperation, OperationToken RightOperation) : OperationToken(Operator)
{
    public static BinaryOperationToken And(OperationToken leftOperation, OperationToken rightOperation) => new(FilterOperator.And, leftOperation, rightOperation);
    public static BinaryOperationToken Or(OperationToken leftOperation, OperationToken rightOperation) => new(FilterOperator.Or, leftOperation, rightOperation);
}