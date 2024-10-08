using System.Text;
using ASK.Filters.Operations;

namespace ASK.Filters;

public class PolishNotationFilterParser(FilterOptions filterOptions, bool reverse = false)
{
    public FilterOptions Options { get; } = filterOptions;

    public Filter Parse(string filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        var tokens = reverse ? TokenizeReverse(filter) : Tokenize(filter);
        return new Filter(filter, GetOperation(tokens));
    }

    public bool TryParse(string filter, out Filter? result)
    {
        try
        {
            result = Parse(filter);
        }
        catch (Exception)
        {
            result = null;
            return false;
        }

        return true;
    }

    private IOperation GetOperation(Stack<string> tokens)
    {
        if (tokens.Count == 0)
            throw new FormatException();

        var operatorType = tokens.Pop().ToUpper();

        if (Options.BinaryOperations.TryGetValue(operatorType, out var binaryOperation))
        {
            var left = GetOperation(tokens);
            var right = GetOperation(tokens);

            return binaryOperation(left, right);
        }

        if (Options.UnaryOperations.TryGetValue(operatorType, out var unaryOperation))
        {
            var operation = GetOperation(tokens);

            return unaryOperation(operation);
        }

        if (Options.PropertyOperations.TryGetValue(operatorType, out var propertyOperation))
        {
            if (tokens.Count < 2)
                throw new FormatException();

            var propertyName = tokens.Pop();
            var value = tokens.Pop();

            var property = Options.GetPropertyByName(propertyName);
            if (property is null)
                throw new FormatException($"Property {propertyName} not found");

            return propertyOperation(property.Name, Options.ConvertToType(value, property.Type));
        }

        throw new FormatException("Invalid operation type");
    }

    private static Stack<string> Tokenize(string input)
    {
        return new Stack<string>(Tokenizer.Tokenize(input).Reverse().ToArray());
    }

    private static Stack<string> TokenizeReverse(string input)
    {
        return new Stack<string>(Tokenizer.Tokenize(input).ToArray());
    }
}