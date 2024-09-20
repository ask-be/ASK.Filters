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
    }

    public FilterOptions(Type type, CultureInfo? cultureInfo = null)
        :this(type.GetProperties().Select(x => new FilterProperty(x.Name, x.PropertyType)), cultureInfo)
    {
    }

    public FilterOptions(object value, CultureInfo? cultureInfo = null)
        :this(value.GetType(), cultureInfo)
    {
    }

    internal FilterProperty? GetPropertyByName(string propertyName)
    {
        return _availableFilterProperties.Find(p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
    }

    public IReadOnlyDictionary<string,CreateBinaryOperationFunc> BinaryOperations => _binaryOperations;
    public IReadOnlyDictionary<string,CreateUnaryOperationFunc> UnaryOperations => _unaryOperations;
    public IReadOnlyDictionary<string,CreatePropertyOperationFunc> PropertyOperations => _propertyOperations;

    public ITokenizer Tokenizer { get; private set; } = new DefaultTokenizer();

    public FilterOptions ClearOperations()
    {
        _binaryOperations.Clear();
        _availableFilterProperties.Clear();
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