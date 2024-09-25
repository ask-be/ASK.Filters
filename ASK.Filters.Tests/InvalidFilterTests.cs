using ASK.Filters.Tests.EntityFramework;

namespace ASK.Filters.Tests;

public class InvalidFilterTests
{
    [Theory]
    [InlineData("Eq")]
    [InlineData("Sum Price 22")]
    [InlineData("Eq Price Hello")]
    [InlineData("Eq Unknown Hello")]
    [InlineData("Eq CreationDate Hello")]
    [InlineData("Eq Id Hello")]
    [InlineData("Eq IsOutOfStock Hello")]
    [InlineData("Ct IsOutOfStock true")]
    [InlineData("Eq IsOutOfStock")]
    [InlineData("And Eq IsOutOfStock")]
    public void InvalidCastFilter(string filter)
    {
        var parser = new PolishNotationFilterParser(new FilterOptions<Product>());
        Assert.Throws<FormatException>(() => parser.Parse(filter));
    }
}