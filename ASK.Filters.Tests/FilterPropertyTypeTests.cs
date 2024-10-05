using System.Runtime.InteropServices.ComTypes;
using ASK.Filters.Operations;
using FluentAssertions;

namespace ASK.Filters.Tests;

public record CustomId(string Value);

public class FilterPropertyTypeTests
{
    private readonly FilterOptions _options;
    private readonly PolishNotationFilterParser _parser;

    public FilterPropertyTypeTests()
    {
        _options = new FilterOptions([
            new FilterProperty<string>("Name"),
            new FilterProperty<int>("Quantity"),
            new FilterProperty<decimal>("Price"),
            new FilterProperty<DateOnly>("BirthDate"),
            new FilterProperty<DateTime>("CreationDate"),
            new FilterProperty<TimeOnly>("Time")
        ]);
        _parser = new PolishNotationFilterParser(_options);
    }

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
    public void ParseEqualStringFilterWithEmpty()
    {
        const string q = "eq name EMPTY";
        var filter = _parser.Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new EqualOperation("Name",string.Empty));
    }

    [Fact]
    public void ParseEqualStringFilterWithCustomEmptyValue()
    {
        const string q = "eq name STRING-EMPTY";
        var filter = new PolishNotationFilterParser(_options.WithStringEmpty("STRING-EMPTY")).Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new EqualOperation("Name",string.Empty));
    }

    [Fact]
    public void ParseEqualStringFilterWithoutEmptyValueSupport()
    {
        const string q = "eq name EMPTY";
        var filter = new PolishNotationFilterParser(_options.WithoutStringEmpty()).Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new EqualOperation("Name","EMPTY"));
    }

    [Fact]
    public void ParseEqualStringFilterWithSpaces()
    {
        const string q = "eq name John_Doe";
        var filter = _parser.Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new EqualOperation("Name","John Doe"));
    }

    [Fact]
    public void ParseEqualStringFilterWithCustomSpacesValue()
    {
        const string q = "eq name John|Doe";
        var filter = new PolishNotationFilterParser(_options.WithWhiteSpace("|")).Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new EqualOperation("Name","John Doe"));
    }
    [Fact]
    public void ParseEqualStringFilterWithoutWhiteSpaceSupport()
    {
        const string q = "eq name John_Doe";
        var filter = new PolishNotationFilterParser(_options.WithoutWhiteSpace()).Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new EqualOperation("Name","John_Doe"));
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
        var parser = new PolishNotationFilterParser(new FilterOptions([
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