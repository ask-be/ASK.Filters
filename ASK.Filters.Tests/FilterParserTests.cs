using ASK.Filters.Operations;
using FluentAssertions;

namespace ASK.Filters.Tests;

public record User(string FirstName, string LastName);

public class FilterParserTests
{
    [Fact]
    public void CanParseAnd()
    {
        var o = new FilterOptions([
            new FilterProperty<string>("firstname"),
            new FilterProperty<string>("lastname")
        ]);

        var andQuery = "and eq firstname John eq lastname Doe";

        var filter = new FilterParser(o).Parse(andQuery);

        filter.Operation.Should().BeAssignableTo<AndOperation>();


        var expression = FilterEvaluator.Default.GetExpression<User>(filter);


    }
}