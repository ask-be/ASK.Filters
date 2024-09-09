using System.Globalization;
using System.Linq.Expressions;
using System.Numerics;
using ASK.Filters.Operations;

namespace ASK.Filters;

public class FilterParser(FilterOptions filterOptions)
{
    public Filter Parse(string filter)
    {
        var tokens = filterOptions.Tokenizer.Tokenize(filter);
        return new Filter(filter, GetOperation(tokens));
    }

    public bool TryParse(string filter, out Filter? result)
    {
        try
        {
            var tokens = filterOptions.Tokenizer.Tokenize(filter);
            result = new Filter(filter, GetOperation(tokens));
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
        if(tokens.Count == 0)
            throw new FormatException();

        var operatorType = tokens.Pop().ToUpper();

        if (filterOptions.BinaryOperations.TryGetValue(operatorType, out var binaryOperation))
        {
            var left = GetOperation(tokens);
            var right = GetOperation(tokens);

            return binaryOperation(left,right);
        }

        if (filterOptions.UnaryOperations.TryGetValue(operatorType, out var unaryOperation))
        {
            var operation = GetOperation(tokens);

            return unaryOperation(operation);
        }

        if (filterOptions.PropertyOperations.TryGetValue(operatorType, out var propertyOperation))
        {
            if(tokens.Count < 2)
                throw new FormatException();

            var propertyName = tokens.Pop();
            var value = tokens.Pop();

            var realPropertyName = filterOptions.GetPropertyByName(propertyName);
            if (realPropertyName is null)
                throw new FormatException($"Property {propertyName} not found");

            var propertyValue = ConvertToType(value, realPropertyName.Type, filterOptions.CultureInfo);

            return propertyOperation(realPropertyName.Name,propertyValue);
        }

        throw new FormatException("Invalid operation type");
    }

    private static object ConvertToType(string value, Type propertyType, CultureInfo cultureInfo)
    {
        return propertyType switch
        {
            not null when propertyType == typeof(string) => value,
            not null when propertyType == typeof(char) => value.Length == 1 ? value[0] : throw new FormatException($"Cannot convert {value} to char"),
            not null when propertyType == typeof(int) => int.Parse(value, cultureInfo),
            not null when propertyType == typeof(long) => long.Parse(value, cultureInfo),
            not null when propertyType == typeof(double) => double.Parse(value, cultureInfo),
            not null when propertyType == typeof(float) => float.Parse(value, cultureInfo),
            not null when propertyType == typeof(Enum) => Enum.Parse(propertyType, value, true),
            not null when propertyType == typeof(decimal) => decimal.Parse(value, cultureInfo),
            not null when propertyType == typeof(DateTime) => DateTime.Parse(value, cultureInfo),
            not null when propertyType == typeof(DateTimeOffset) => DateTimeOffset.Parse(value, cultureInfo),
            not null when propertyType == typeof(DateOnly) => DateOnly.Parse(value, cultureInfo),
            not null when propertyType == typeof(TimeOnly) => TimeOnly.Parse(value, cultureInfo),
            not null when propertyType == typeof(bool) => ConvertBool(value),
            _ => throw new Exception($"Cannot convert {value} to {propertyType}")
        };
    }

    private static bool ConvertBool(string value){
        var valueToLower = value.ToLower();
        return valueToLower is "true" or "1" || (valueToLower is "false" or "0"
            ? false
            : throw new FormatException($"Cannot convert {value} to bool"));
    }
}