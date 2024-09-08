namespace ASK.Filters.OperationTokens;

internal record PropertyOperationToken(FilterOperator Operator, string Name, object Value) : OperationToken(Operator)
{
    public static PropertyOperationToken Equal(string name, object value)
    {
        return new PropertyOperationToken(FilterOperator.Equal, name, value);
    }
    public static PropertyOperationToken GreaterThan(string name, object value)
    {
        if(value is string)
            throw new FormatException("GreaterThan value cannot be a string");

        return new PropertyOperationToken(FilterOperator.GreaterThan, name, value);
    }
    public static PropertyOperationToken GreaterThanOrEqual(string name, object value)
    {
        if(value is string)
            throw new FormatException("GreaterThanOrEqual value cannot be a string");

        return new PropertyOperationToken(FilterOperator.GreaterThanOrEqual, name, value);
    }
    public static PropertyOperationToken LessThan(string name, object value)
    {
        if(value is string)
            throw new FormatException("LessThan value cannot be a string");


        return new PropertyOperationToken(FilterOperator.LessThan, name, value);
    }
    public static PropertyOperationToken LessThanOrEqual(string name, object value)
    {
        if(value is string)
            throw new FormatException("LessThanOrEqual value cannot be a string");

        return new PropertyOperationToken(FilterOperator.LessThanOrEqual, name, value);
    }
    public static PropertyOperationToken Contains(string name, object value)
    {
        if(value is not string)
            throw new FormatException("Contains value must be a string");

        return new PropertyOperationToken(FilterOperator.Contains, name, value);
    }
    public static PropertyOperationToken StartWith(string name, object value)
    {
        if(value is not string)
            throw new FormatException("StartWith value must be a string");

        return new PropertyOperationToken(FilterOperator.StartWith, name, value);
    }
    public static PropertyOperationToken EndWith(string name, object value)
    {
        if(value is not string)
            throw new FormatException("EndWith value must be a string");

        return new PropertyOperationToken(FilterOperator.EndWith, name, value);
    }
}