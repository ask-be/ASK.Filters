# ASK.Filters

**ASK.Filters** is a C# library that converts `WHERE` clauses written in Polish notation into expressions that can be used with EntityFramework or any IEnumerable. This library is particularly useful for REST APIs, allowing complex filters to be specified directly in the URL.

## Features

- Converts `WHERE` clauses in Polish notation to LINQ expressions.
- Operations AND, OR, NOT, EQ, GT, GTE, LT, LTE, CONTAINS, START and END included
- No dependencies between API and Infrastructure layers.
- Easily configurable:
  - Add custom operations.
  - Add custom converters (useful with ValueType mapping in Entity Framework).
  - Use custom evaluators to generate complex expressions if required.

## Installation

Add the library to your project via NuGet:

```bash
Install-Package ASK.Filters
```

## Usage

### Query Example

For a collection of resources `/books`, you can specify a filter in the URL:

```
/books?q=AND CONTAINS author John EQUAL publicationyear 1998
```

This query will return books where the author's name contains "John" and the publication year is 1998.

### Code Example

Here's how to use the library in your C# project:

```csharp
using ASK.Filters;
using System.Linq.Expressions;

// Example of a WHERE clause in Polish notation
string query = "AND CONTAINS author John EQUAL publicationyear 1998";

// Create FilterOptions that contain all the available values and operations of the filter 
var filterOptions = new FilterOptions([
    new FilterProperty<string>("author"),
    new FilterProperty<int>("publicationyear")
]);

// Once you have filter options, create a parser
var filterParser = new FilterParser(filterOptions);

// Parse your filter
var filter = filterParser.Parse(query);

var filterEvaluator = new FilterEvaluator<Book>();
var expression  = filterEvaluator.GetExpression(filter);

// Use with EntityFramework
using (var context = new BookContext())
{
    var books = context.Books.Where(expression).ToList();
}
```

## Customizing Query Generation

ASK.Filters allows for advanced customization of query generation. By default, the library maps a filter property directly to a property on the object being filtered. However, you can extend this behavior by adding custom operations, specific converters (particularly useful for ValueType mapping in Entity Framework), and custom evaluators to generate complex expressions as needed.

### Example of Custom Operation

To create a custom operation, such as the `LIKE` method in EntityFramework, start by creating the `LikeOperation` class inheriting from `PropertyOperation`:

```csharp
public record LikeOperation : PropertyOperation
{
    private static readonly MethodInfo LikeMethod = typeof(DbFunctionsExtensions)
        .GetMethod(nameof(DbFunctionsExtensions.Like), new[] { typeof(DbFunctions), typeof(string), typeof(string) })!;
    private static readonly Expression EfFunctions = Expression.Constant(EF.Functions);

    public LikeOperation(string name, object value) : base(name, value)
    {
        if (value is not string)
            throw new FormatException("Like value must be a string");
    }

    public override Expression GetExpression(Expression left, Expression right)
    {
        return Expression.Call(LikeMethod, EfFunctions, left, right);
    }
}
```

Next, add your custom operation to the `FilterOptions`:

```csharp
var options = new FilterOptions(...)
    .AddOperation("LIKE", (name, value) => new LikeOperation(name, value));
```

You can now use the `LIKE` keyword in your filters:

```
LIKE Name V?nc%
```
### Example of Custom Evaluator
Let’s say you want a filter on the FirstName of a User to match either the FirstName or the PhoneticFirstName properties. You can achieve this by creating a custom evaluator:

```csharp
public class UserFilterEvaluator : FilterEvaluator<User>
{
    protected override Expression GetPropertyExpression(ParameterExpression parameter, PropertyOperation property)
    {
        if (property.Name == "FirstName")
        {
            return Expression.Or(
                property.GetExpression(
                    Expression.Property(parameter, "Firstname"),
                    Expression.Constant(property.Value)
                ), property.GetExpression(
                    Expression.Property(parameter, "PhoneticFirstName"),
                    Expression.Constant(property.Value)
                ));
        }
        return base.GetPropertyExpression(parameter, property);
    }
}
```

To use this custom evaluator, you would create an instance and get the expression as follows:

```csharp
var expression  = new UserFilterEvaluator().GetExpression(filter);
```

## License

This project is licensed under the LGPL License.
