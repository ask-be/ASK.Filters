using ASK.Filters.Operations;

namespace ASK.Filters;

public record Filter
{
    public Filter(string Value, IOperation Operation)
    {
        ArgumentNullException.ThrowIfNull(Value);
        ArgumentNullException.ThrowIfNull(Operation);
        
        this.Value = Value;
        this.Operation = Operation;
    }

    public string Value { get; }
    public IOperation Operation { get; }

    public void Deconstruct(out string value, out IOperation operation)
    {
        value = Value;
        operation = Operation;
    }
}