using ASK.Filters.Tests.EntityFramework;
using FluentAssertions;

namespace ASK.Filters.Tests;

public class FilterExpressionBuilderTests
{
    [Fact]
    public void CanFilterProductByPrice()
    {
        var parser = new PolishNotationFilterParser(new FilterOptions<Product>());
        var filter = parser.Parse("gt price 150");
        Product.SampleValues.ApplyFilter(filter).Count().Should().Be(17);
    }

}