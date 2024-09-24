using ASK.Filters.Tests.EntityFramework;
using FluentAssertions;

namespace ASK.Filters.Tests;

public class FilterExpressionBuilderTests
{
    [Fact]
    public void CanFilterProductByPrice()
    {
        var parser = new FilterPolishNotationParser(new FilterOptions<Product>());
        var filter = parser.Parse("gt price 150");
        Product.SampleValues.ApplyFilter(filter).Count().Should().Be(17);
    }

}