using System.Globalization;
using ASK.Filters.Operations;
using ASK.Filters.Tokenizers;

namespace ASK.Filters;

public delegate IOperation CreateBinaryOperationFunc(IOperation left, IOperation right);
public delegate IOperation CreateUnaryOperationFunc(IOperation operation);
public delegate IOperation CreatePropertyOperationFunc(string name, object value);

public record FilterProperty(string Name, Type Type);

public record FilterProperty<T>(string Name) : FilterProperty(Name, typeof(T));

public class FilterOptions
{
    private readonly Dictionary<string, CreateBinaryOperationFunc> _binaryOperations = new();
    private readonly Dictionary<string, CreateUnaryOperationFunc> _unaryOperations = new();
    private readonly Dictionary<string, CreatePropertyOperationFunc> _propertyOperations = new();

    public CultureInfo CultureInfo { get; }

    private readonly List<FilterProperty> _availableFilterProperties;
    private readonly Dictionary<Type, Func<string, object>> _converters = new();

    public FilterOptions(IEnumerable<FilterProperty> properties, CultureInfo? cultureInfo = null)
    {
        _availableFilterProperties = properties.ToList();
        if(_availableFilterProperties.Count == 0)
            throw new ArgumentException("At least one filter property is required.");

        CultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;

        AddOperation("AND", (x,y) => new AndOperation(x,y));
        AddOperation("OR", (x,y) => new OrOperation(x,y));

        AddOperation("NOT", x => new NotOperation(x));

        AddOperation("EQ", (x,y) => new EqualOperation(x,y));
        AddOperation("GT", (x,y) => new GreaterThanOperation(x,y));
        AddOperation("GTE", (x,y) => new GreaterThanOrEqualOperation(x,y));
        AddOperation("LT", (x,y) => new LessThanOperation(x,y));
        AddOperation("LTE", (x,y) => new LessThanOrEqualOperation(x,y));
        AddOperation("CT", (x,y) => new ContainsOperation(x,y));
        AddOperation("SW", (x,y) => new StartWithOperation(x,y));
        AddOperation("EW", (x,y) => new EndWithOperation(x,y));

        AddConverter<string>(x => x);
        AddConverter(x => x.Length == 1 ? x[0] : throw new FormatException($"Cannot convert {x} to char"));
        AddConverter(x => int.Parse(x, CultureInfo));
        AddConverter(x => long.Parse(x, CultureInfo));
        AddConverter(x => float.Parse(x, CultureInfo));
        AddConverter(x => double.Parse(x, CultureInfo));
        AddConverter(x => decimal.Parse(x, CultureInfo));
        AddConverter(x => DateTime.Parse(x, CultureInfo));
        AddConverter(x => DateTimeOffset.Parse(x, CultureInfo));
        AddConverter(x => DateOnly.Parse(x, CultureInfo));
        AddConverter(x => TimeOnly.Parse(x, CultureInfo));
        AddConverter(x => TimeSpan.Parse(x, CultureInfo));
        AddConverter(x =>
        {
            var valueToLower = x.ToLower();
            return valueToLower is "true" or "1" || (valueToLower is "false" or "0"
                ? false
                : throw new FormatException($"Cannot convert {x} to bool"));
        });
    }

    public FilterOptions(Type type, CultureInfo? cultureInfo = null)
        :this(type.GetProperties().Select(x => new FilterProperty(x.Name, x.PropertyType)), cultureInfo)
    {
    }

    internal FilterProperty? GetPropertyByName(string propertyName)
    {
        return _availableFilterProperties.Find(p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
    }

    internal object ConvertToType(string value, Type type)
    {
        if (_converters.TryGetValue(type, out var converter))
        {
            return converter(value);
        }
        throw new FormatException($"Cannot convert {value} to type {type}");
    }

    public IReadOnlyDictionary<string,CreateBinaryOperationFunc> BinaryOperations => _binaryOperations;
    public IReadOnlyDictionary<string,CreateUnaryOperationFunc> UnaryOperations => _unaryOperations;
    public IReadOnlyDictionary<string,CreatePropertyOperationFunc> PropertyOperations => _propertyOperations;

    public ITokenizer Tokenizer { get; private set; } = new DefaultTokenizer();
    public IReadOnlyList<FilterProperty> FilterProperties => _availableFilterProperties;

    public FilterOptions AddConverter<T>(Func<string, T> converter)
    {
        _converters[typeof(T)] = input => converter(input)!;
        return this;
    }

    public FilterOptions ClearOperations()
    {
        _binaryOperations.Clear();
        _unaryOperations.Clear();
        _propertyOperations.Clear();
        return this;
    }

    public FilterOptions AddOperation(string name, CreateBinaryOperationFunc createOperation)
    {
        _binaryOperations.Add(name.ToUpper(), createOperation);
        return this;
    }
    public FilterOptions AddOperation(string name, CreateUnaryOperationFunc createOperation)
    {
        _unaryOperations.Add(name.ToUpper(), createOperation);
        return this;
    }
    public FilterOptions AddOperation(string name, CreatePropertyOperationFunc createOperation)
    {
        _propertyOperations.Add(name.ToUpper(), createOperation);
        return this;
    }

    public FilterOptions WithTokenizer(ITokenizer tokenizer)
    {
        Tokenizer = tokenizer;
        return this;
    }

    public FilterOptions AddProperty<T>(string name)
    {
        _availableFilterProperties.Add(new FilterProperty(name, typeof(T)));
        return this;
    }
}

public class FilterOptions<T>(CultureInfo? cultureInfo = null) : FilterOptions(typeof(T), cultureInfo);