using System.Globalization;
using ASK.Filters.OperationTokens;
using FluentAssertions;

namespace ASK.Filters.Tests;

public class FilterTests
{
    [Fact]
    public void ParseSimpleEqualFilter()
    {
        var filter = Filter.Parse("eq name Vincent", new FilterOptions([
            new FilterProperty<string>("Name","n"),
            new FilterProperty<string>("FirstName","fn")
        ]));

        filter.OperationToken.Should().Be(new PropertyOperationToken(FilterOperator.Equal,"Name","Vincent"));
    }

    [Fact]
    public void ParseAndFilter()
    {
        var filter = Filter.Parse("and eq name Vincent gt birthdate 2020-12-10", new FilterOptions(new {Name = "", Birthdate = DateTime.MinValue}));

        filter.OperationToken.Should().Be(
            new BinaryOperationToken(FilterOperator.And,
                new PropertyOperationToken(FilterOperator.Equal, "Name", "Vincent"),
                new PropertyOperationToken(FilterOperator.GreaterThan, "Birthdate", new DateTime(2020, 12, 10))));
    }
}