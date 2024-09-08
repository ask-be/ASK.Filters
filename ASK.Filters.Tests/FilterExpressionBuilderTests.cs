using ASK.Filters.Tests.EntityFramework;
using FluentAssertions;

namespace ASK.Filters.Tests;

public class FilterExpressionBuilderTests
{
    [Fact]
    public void CanFilterProductByPrice()
    {
        var filter = Filter.Parse("gt price 150", new FilterOptions<Product>());
        Product.SampleValues.ApplyFilter(filter).Count().Should().Be(16);
    }

}