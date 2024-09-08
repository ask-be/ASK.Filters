namespace ASK.Filters.OperationTokens;

internal record UnaryOperationToken(FilterOperator Operator, OperationToken Operation) : OperationToken(Operator)
{
    public static UnaryOperationToken Not(OperationToken operation) => new(FilterOperator.Not, operation);
}