# ASK.Filters

**ASK.Filters** is a C# library that converts `WHERE` clauses written in Polish notation into expressions that can be used with EntityFramework or any IEnumerable. This library is particularly useful for REST APIs, allowing complex filters to be specified directly in the URL.

## Features

- Converts `WHERE` clauses in Polish notation to LINQ expressions.
- Supports logical operators `AND` and `OR` without the need for parentheses.
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

// Use with EntityFramework
using (var context = new BookContext())
{
    var books = context.Books.ApplyFilter(filter).ToList();
}
```

## License

This project is licensed under the LGPL License.
