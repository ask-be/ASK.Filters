using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ASK.Filters.OperationTokens;

[assembly:InternalsVisibleTo("ASK.Filters.Tests")]

namespace ASK.Filters;

public class Filter
{
    private Filter(OperationToken operationToken)
    {
        OperationToken = operationToken;
    }

    public static Filter Parse(string filter, FilterOptions filterOptions)
    {
        return new Filter(ReadOperationTokens(FilterTokenizer.Tokenize(filter), filterOptions));
    }

    public static bool TryParse(string filter, FilterOptions filterOptions, out Filter? result)
    {
        try
        {
            result = new Filter(ReadOperationTokens(FilterTokenizer.Tokenize(filter), filterOptions));
        }
        catch (Exception)
        {
            result = null;
            return false;
        }

        return true;
    }

    internal OperationToken OperationToken { get; private set; }

    internal Expression<Func<T, bool>> CreateExpression<T>()
    {
        return FilterExpressionBuilder.BuildExpression<T>(this);
    }

    private static OperationToken ReadOperationTokens(Stack<string> tokens, FilterOptions filterOptions)
    {
        if(tokens.Count == 0)
            throw new FormatException("Expect at least one token more.");

        var operatorType = tokens.Pop().ToUpper();
        switch (operatorType)
        {
            case "AND":
                return BinaryOperationToken.And(ReadOperationTokens(tokens,filterOptions), ReadOperationTokens(tokens,filterOptions));
            case "OR":
                return BinaryOperationToken.Or(ReadOperationTokens(tokens,filterOptions), ReadOperationTokens(tokens,filterOptions));
            case "NOT":
                return UnaryOperationToken.Not(ReadOperationTokens(tokens,filterOptions));
            default:
                if(tokens.Count < 2)
                    throw new FormatException("Expect at least two more token.");

                var propertyName = tokens.Pop();
                var value = tokens.Pop();

                var realPropertyName = filterOptions.GetPropertyByName(propertyName);
                if (realPropertyName is null)
                    throw new FormatException($"Property {propertyName} not found");

                var propertyValue = ConvertToType(value, realPropertyName.Type, filterOptions.CultureInfo);

                return operatorType switch
                {
                    "EQ" => PropertyOperationToken.Equal(realPropertyName.Name, propertyValue),
                    "GT" => PropertyOperationToken.GreaterThan(realPropertyName.Name, propertyValue),
                    "GTE" => PropertyOperationToken.GreaterThanOrEqual(realPropertyName.Name, propertyValue),
                    "LT" => PropertyOperationToken.LessThan(realPropertyName.Name, propertyValue),
                    "LTE" => PropertyOperationToken.LessThanOrEqual(realPropertyName.Name, propertyValue),
                    "CT" => PropertyOperationToken.Contains(realPropertyName.Name, propertyValue),
                    "SW" => PropertyOperationToken.StartWith(realPropertyName.Name, propertyValue),
                    "EW" => PropertyOperationToken.EndWith(realPropertyName.Name, propertyValue),
                    _ => throw new FormatException($"Unsupported operator {operatorType}")
                };
        }
    }

    private static object ConvertToType(string value, Type propertyType, CultureInfo cultureInfo)
    {
        return propertyType switch
        {
            not null when propertyType == typeof(string) => value,
            not null when propertyType == typeof(char) => value.Length == 1 ? value[0] : throw new FormatException($"Cannot convert {value} to char"),
            not null when propertyType == typeof(int) => int.Parse(value, cultureInfo),
            not null when propertyType == typeof(long) => long.Parse(value, cultureInfo),
            not null when propertyType == typeof(Enum) => Enum.Parse(propertyType, value, true),
            not null when propertyType == typeof(decimal) => decimal.Parse(value, cultureInfo),
            not null when propertyType == typeof(DateTime) => DateTime.Parse(value, cultureInfo),
            not null when propertyType == typeof(DateOnly) => DateOnly.Parse(value, cultureInfo),
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