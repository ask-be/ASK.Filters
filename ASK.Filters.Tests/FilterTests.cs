using ASK.Filters.Operations;
using ASK.Filters.Tests.EntityFramework;
using FluentAssertions;

namespace ASK.Filters.Tests;

public class FilterTests
{
    [Fact]
    public void ParseSimpleEqualFilter()
    {
        var parser = new FilterParser(new FilterOptions([
            new FilterProperty<string>("Name"),
            new FilterProperty<string>("FirstName")
        ]));

        var options = new FilterOptions<Product>();
        options.AddOperation("LIKE", (x,y) => new EqualOperation(x,y));

        var filter = parser.Parse("eq name Vincent");

        filter.Operation.Should().Be(new EqualOperation("Name","Vincent"));
    }

    [Fact]
    public void ParseAndFilter()
    {
        var parser = new FilterParser(new FilterOptions([
            new FilterProperty<string>("Name"),
            new FilterProperty<DateTime>("Birthdate")
        ]));

        var filter = parser.Parse("and eq name Vincent gt birthdate 2020-12-10");

        filter.Operation.Should().Be(
            new AndOperation(
                new EqualOperation("Name", "Vincent"),
                new GreaterThanOperation("Birthdate", new DateTime(2020, 12, 10))));
    }
}