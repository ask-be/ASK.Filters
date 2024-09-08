using FluentAssertions;
using Xunit.Abstractions;

namespace ASK.Filters.Tests.EntityFramework;

public class OperatorFilterTests(ITestOutputHelper output) : BaseEFTest(output)
{
    private void ApplyFilterAndCheckCount(string filterString, int expectedCount)
    {
        var filter = Filter.Parse(filterString, new FilterOptions<Product>());
        using var context = GetContext();
        context.Products.ApplyFilter(filter).Count().Should().Be(expectedCount);
    }

    [Theory]
    [InlineData("eq Id 10",1)]
    [InlineData("eq Name Smartwatch",1)]
    [InlineData("eq Price 149.99",2)]
    [InlineData("eq IsOutOfStock false",13)]
    [InlineData("eq CreationDate 2023-12-30",3)]
    public void TestEqualOperatorFilter(string filterString, int expectedCount)
    {
        ApplyFilterAndCheckCount(filterString, expectedCount);
    }

    [Theory]
    [InlineData("not eq Id 10",24)]
    [InlineData("not eq Name Smartwatch",24)]
    [InlineData("not eq Price 149.99",23)]
    [InlineData("not eq IsOutOfStock false",12)]
    [InlineData("not eq CreationDate 2023-12-30",22)]
    public void TestNotOperatorFilter(string filterString, int expectedCount)
    {
        ApplyFilterAndCheckCount(filterString, expectedCount);
    }

    [Theory]
    [InlineData("gt Id 10",15)]
    [InlineData("gt Price 199.99",13)]
    [InlineData("gt CreationDate 2023-12-15",14)]
    public void TestGreaterThanOperatorFilter(string filterString, int expectedCount)
    {
        ApplyFilterAndCheckCount(filterString, expectedCount);
    }

    [Theory]
    [InlineData("gte Id 10",16)]
    [InlineData("gte Price 199.99",16)]
    [InlineData("gte CreationDate 2023-12-30",14)]
    public void TestGreaterThanOrEqualOperatorFilter(string filterString, int expectedCount)
    {
        ApplyFilterAndCheckCount(filterString, expectedCount);
    }

    [Theory]
    [InlineData("lt Id 10",9)]
    [InlineData("lt Price 199.99",9)]
    [InlineData("lt CreationDate 2023-12-15",11)]
    public void TestLessThanOperatorFilter(string filterString, int expectedCount)
    {
        ApplyFilterAndCheckCount(filterString, expectedCount);
    }

    [Theory]
    [InlineData("lte Id 10",10)]
    [InlineData("lte Price 199.99",12)]
    [InlineData("lte CreationDate 2023-12-30",14)]
    public void TestLessThanOrEqualOperatorFilter(string filterString, int expectedCount)
    {
        ApplyFilterAndCheckCount(filterString, expectedCount);
    }

    [Theory]
    [InlineData("Ct Name Wireless",2)]
    public void TestContainsOperatorFilter(string filterString, int expectedCount)
    {
        ApplyFilterAndCheckCount(filterString, expectedCount);
    }
}