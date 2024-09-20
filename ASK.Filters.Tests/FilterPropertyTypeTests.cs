using ASK.Filters.Operations;
using FluentAssertions;

namespace ASK.Filters.Tests;

public record CustomId(string Value);

public class FilterPropertyTypeTests
{
    private readonly FilterParser _parser = new (new FilterOptions([
        new FilterProperty<string>("Name"),
        new FilterProperty<int>("Quantity"),
        new FilterProperty<decimal>("Price"),
        new FilterProperty<DateOnly>("BirthDate"),
        new FilterProperty<DateTime>("CreationDate"),
        new FilterProperty<TimeOnly>("Time")
    ]));

    [Theory]
    [InlineData("eq name Vincent")]
    [InlineData("Eq NamE Vincent")]
    [InlineData("EQ NAME Vincent")]
    public void ParseIsCaseInsensitive(string query)
    {
        var filter = _parser.Parse(query);

        filter.Value.Should().Be(query);
        filter.Operation.Should().Be(new EqualOperation("Name","Vincent"));
    }

    [Fact]
    public void ParseEqualStringFilter()
    {
        const string q = "eq name Vincent";
        var filter = _parser.Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new EqualOperation("Name","Vincent"));
    }

    [Fact]
    public void ParseEqualIntFilter()
    {
        const string q = "eq quantity 200";
        var filter = _parser.Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new EqualOperation("Quantity",200));
    }

    [Fact]
    public void ParseEqualDecimalFilter()
    {
        const string q = "eq price 24.10";
        var filter = _parser.Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new EqualOperation("Price",24.10m));
    }

    [Fact]
    public void ParseEqualDateTimeFilter()
    {
        const string q = "eq creationdate 2024-04-17T23:43:12";
        var filter = _parser.Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new EqualOperation("CreationDate",new DateTime(2024,04,17, 23,43,12, DateTimeKind.Local)));
    }

    [Fact]
    public void ParseEqualDateOnlyFilter()
    {
        const string q = "eq birthdate 2022-03-19";
        var filter = _parser.Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new EqualOperation("BirthDate",new DateOnly(2022,03,19)));
    }

    [Fact]
    public void ParseEqualTimeOnlyFilter()
    {
        const string q = "eq time 23:34:21";
        var filter = _parser.Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new EqualOperation("Time",new TimeOnly(23,34,21)));
    }

    [Fact]
    public void ParseEqualCustomTypeFilter()
    {
        _parser.Options.AddProperty<CustomId>("CustomId");
        _parser.Options.AddConverter(x => new CustomId(x));

        const string q = "eq customId A234534";
        var filter = _parser.Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new EqualOperation("CustomId",new CustomId("A234534")));
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
                new GreaterThanOperation("Birthdate", new DateTime(2020, 12, 10, 0,0,0,DateTimeKind.Local))));
    }
}