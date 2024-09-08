using System.Globalization;

namespace ASK.Filters;

public record FilterProperty(string Name, Type Type, params string[] Aliases);

public record FilterProperty<T>(string Name, params string[] Aliases) : FilterProperty(Name, typeof(T), Aliases);

public class FilterOptions
{
    public CultureInfo CultureInfo { get; }

    private readonly List<FilterProperty> _availableFilterProperties;

    public FilterOptions(IEnumerable<FilterProperty> properties, CultureInfo? cultureInfo = null)
    {
        _availableFilterProperties = properties.ToList();
        if(_availableFilterProperties.Count == 0)
            throw new ArgumentException("At least one filter property is required.");

        CultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
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
        return _availableFilterProperties.Find(p =>
            p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase) ||
            p.Aliases.Contains(propertyName));
    }
}

public class FilterOptions<T>(CultureInfo? cultureInfo = null) : FilterOptions(typeof(T), cultureInfo);