using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ASK.Filters.Operations;

[assembly:InternalsVisibleTo("ASK.Filters.Tests")]

namespace ASK.Filters;

public class Filter(string value, IOperation operation)
{
    public string Value { get; private set; } = value;

    public IOperation Operation { get; private set; } = operation;
}