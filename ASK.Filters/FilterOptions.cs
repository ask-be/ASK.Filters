using System.Globalization;
using ASK.Filters.Operations;

namespace ASK.Filters;

public record FilterProperty
{
    public FilterProperty(string name, Type type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        Type = type;
    }

    public string Name { get; }
    public Type Type { get; }
}

public record FilterProperty<T>(string Name) : FilterProperty(Name, typeof(T));

public class FilterOptions
{
    private readonly Dictionary<string, Func<IOperation,IOperation,IOperation>> _binaryOperations = new();
    private readonly Dictionary<string, Func<IOperation,IOperation>> _unaryOperations = new();
    private readonly Dictionary<string, Func<string,object?,IOperation>> _propertyOperations = new();

    private readonly List<FilterProperty> _availableFilterProperties;
    private readonly Dictionary<Type, Func<string, object>> _converters = new();

    /// <summary>
    /// CultureInfo used while converting string value to property types.
    /// </summary>
    public CultureInfo CultureInfo { get; }

    /// <summary>
    /// Value that must be considered as NULL
    /// Default value is NULL.
    /// </summary>
    public string? NullValue { get; private set; }

    /// <summary>
    /// String value that must be considered as empty string.
    /// Default value is EMPTY.
    /// </summary>
    public string? StringEmptyValue { get; private set;}

    public FilterOptions(IEnumerable<FilterProperty> properties, CultureInfo? cultureInfo = null)
    {
        ArgumentNullException.ThrowIfNull(properties);

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
        AddOperation("CONTAINS", (x,y) => new ContainsOperation(x,y));
        AddOperation("START", (x,y) => new StartWithOperation(x,y));
        AddOperation("END", (x,y) => new EndWithOperation(x,y));

        AddConverter<string>(x => x == StringEmptyValue ? string.Empty : x);
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

    internal object? ConvertToType(string value, Type type)
    {
        if (value == NullValue)
            return null;

        if (_converters.TryGetValue(type, out var converter))
        {
            return converter(value);
        }
        throw new FormatException($"Cannot convert {value} to type {type}");
    }

    public IReadOnlyDictionary<string,Func<IOperation,IOperation,IOperation>> BinaryOperations => _binaryOperations;
    public IReadOnlyDictionary<string,Func<IOperation,IOperation>> UnaryOperations => _unaryOperations;
    public IReadOnlyDictionary<string,Func<string,object?,IOperation>> PropertyOperations => _propertyOperations;

    public IReadOnlyList<FilterProperty> FilterProperties => _availableFilterProperties;

    /// <summary>
    /// Add a converter from string to a Type used by a FilterProperty.
    /// </summary>
    /// <param name="converter"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Current FilterOptions for chaining</returns>
    public FilterOptions AddConverter<T>(Func<string, T> converter)
    {
        ArgumentNullException.ThrowIfNull(converter);

        _converters[typeof(T)] = input => converter(input)!;
        return this;
    }

    /// <summary>
    /// Remove all registered operations.
    /// </summary>
    /// <returns>Current FilterOptions for chaining</returns>
    public FilterOptions ClearOperations()
    {
        _binaryOperations.Clear();
        _unaryOperations.Clear();
        _propertyOperations.Clear();
        return this;
    }

    /// <summary>
    /// Add a Binary Operation that combines two other operations such as AND and OR.
    /// </summary>
    /// <param name="name">Name of the operation in the filter</param>
    /// <param name="createOperation">Factory to create the Operation</param>
    /// <returns>Current FilterOptions for chaining</returns>
    public FilterOptions AddOperation(string name, Func<IOperation,IOperation,IOperation> createOperation)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(createOperation);

        _binaryOperations.Add(name.ToUpper(), createOperation);
        return this;
    }

    /// <summary>
    /// Add an Unary Operation that combines two other operations such as NOT.
    /// </summary>
    /// <param name="name">Name of the operation in the filter</param>
    /// <param name="createOperation">Factory to create the Operation</param>
    /// <returns>Current FilterOptions for chaining</returns>
    public FilterOptions AddOperation(string name, Func<IOperation,IOperation> createOperation)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(createOperation);

        _unaryOperations.Add(name.ToUpper(), createOperation);
        return this;
    }

    /// <summary>
    /// Add a Operation that can be performed on Property such as Equal, GreaterThan, ...
    /// </summary>
    /// <param name="name">Name of the operation in the filter</param>
    /// <param name="createOperation">Factory to create the Operation</param>
    /// <returns>Current FilterOptions for chaining</returns>
    public FilterOptions AddOperation(string name, Func<string,object?,IOperation> createOperation)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(createOperation);

        _propertyOperations.Add(name.ToUpper(), createOperation);
        return this;
    }

    /// <summary>
    /// Specify the value that must be considered as NULL
    /// </summary>
    /// <param name="nullValue">The NULL value</param>
    /// <returns>Current FilterOptions for chaining</returns>
    public FilterOptions WithNullValue(string nullValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nullValue);

        NullValue = nullValue;
        return this;
    }

    /// <summary>
    /// Remove support for NULL value
    /// </summary>
    /// <returns>Current FilterOptions for chaining</returns>
    public FilterOptions WithoutNullValue()
    {
        NullValue = null;
        return this;
    }

    /// <summary>
    /// Specify the value that must be considered as an Empty String
    /// <remarks>Only usable for Properties of type string</remarks>
    /// </summary>
    /// <returns>Current FilterOptions for chaining</returns>
    public FilterOptions WithStringEmpty(string stringEmptyValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(stringEmptyValue);

        StringEmptyValue = stringEmptyValue;
        return this;
    }

    /// <summary>
    /// Remove support for Empty strings
    /// </summary>
    /// <returns>Current FilterOptions for chaining</returns>
    public FilterOptions WithoutStringEmpty()
    {
        StringEmptyValue = null;
        return this;
    }

    /// <summary>
    /// Add a Property that can be used in the filter
    /// </summary>
    /// <param name="name">Property Name</param>
    /// <typeparam name="T">Property Type</typeparam>
    /// <returns>Current FilterOptions for chaining</returns>
    public FilterOptions AddProperty<T>(string name)
    {
        _availableFilterProperties.Add(new FilterProperty(name, typeof(T)));
        return this;
    }
}

public class FilterOptions<T>(CultureInfo? cultureInfo = null) : FilterOptions(typeof(T), cultureInfo);