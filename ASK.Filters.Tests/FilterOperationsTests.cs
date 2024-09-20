using ASK.Filters.Operations;
using FluentAssertions;

namespace ASK.Filters.Tests;


public class FilterOperationsTests
{
    private readonly FilterParser _parser = new (new FilterOptions([
        new FilterProperty<string>("Name"),
        new FilterProperty<int>("Quantity"),
        new FilterProperty<decimal>("Price"),
        new FilterProperty<DateOnly>("BirthDate"),
        new FilterProperty<DateTime>("CreationDate"),
        new FilterProperty<TimeOnly>("Time")
    ]));

    [Fact]
    public void ParseEqualStringFilter()
    {
        const string q = "eq name Vincent";
        var filter = _parser.Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new EqualOperation("Name","Vincent"));
    }

    [Fact]
    public void ParseNotEqualStringFilter()
    {
        const string q = "not eq name Vincent";
        var filter = _parser.Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new NotOperation(new EqualOperation("Name","Vincent")));
    }

    [Fact]
    public void ParseGreaterThanFilter()
    {
        const string q = "gt quantity 10";
        var filter = _parser.Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new GreaterThanOperation("Quantity",10));
    }

    [Fact]
    public void ParseGreaterThanOrEqualFilter()
    {
        const string q = "gte quantity 15";
        var filter = _parser.Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new GreaterThanOrEqualOperation("Quantity",15));
    }

    [Fact]
    public void ParseLessThanFilter()
    {
        const string q = "lt quantity 10";
        var filter = _parser.Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new LessThanOperation("Quantity",10));
    }

    [Fact]
    public void ParseLessThanOrEqualFilter()
    {
        const string q = "lte quantity 15";
        var filter = _parser.Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new LessThanOrEqualOperation("Quantity",15));
    }

    [Fact]
    public void ParseContainsFilter()
    {
        const string q = "contains name hel";
        var filter = _parser.Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new ContainsOperation("Name","hel"));
    }

    [Fact]
    public void ParseStartWithFilter()
    {
        const string q = "start name aa";
        var filter = _parser.Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new StartWithOperation("Name","aa"));
    }

    [Fact]
    public void ParseEndWithFilter()
    {
        const string q = "end name ww";
        var filter = _parser.Parse(q);

        filter.Value.Should().Be(q);
        filter.Operation.Should().Be(new EndWithOperation("Name","ww"));
    }

    [Fact]
    public void ParseAndFilter()
    {
        var filter = _parser.Parse("and eq name Vincent gt birthdate 2020-12-10");

        filter.Operation.Should().Be(
            new AndOperation(
                new EqualOperation("Name", "Vincent"),
                new GreaterThanOperation("BirthDate", new DateOnly(2020, 12, 10))));
    }

    [Fact]
    public void ParseOrFilter()
    {
        var filter = _parser.Parse("or eq name Bob gt birthdate 2022-11-12");

        filter.Operation.Should().Be(
            new OrOperation(
                new EqualOperation("Name", "Bob"),
                new GreaterThanOperation("BirthDate", new DateOnly(2022, 11, 12))));
    }

    [Fact]
    public void ParseOrAndAndFilter()
    {
        var filter = _parser.Parse("and or eq name Bob gt birthdate 2022-11-12 lt price 20");

        filter.Operation.Should().Be(
            new AndOperation(
            new OrOperation(
                new EqualOperation("Name", "Bob"),
                new GreaterThanOperation("BirthDate", new DateOnly(2022, 11, 12))),
            new LessThanOperation("Price", 20m)));
    }
}