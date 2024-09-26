using System.Linq.Expressions;
using ASK.Filters.Operations;
using FluentAssertions;
using Xunit.Abstractions;

namespace ASK.Filters.Tests.EntityFramework;

public class CustomProductFilterEvaluator : FilterEvaluator<Product>
{
    public CustomProductFilterEvaluator()
    {
        AddOperationEvaluator<LikeOperation>(new LikeOperationEvaluator());
    }

    protected override Expression EvaluatePropertyOperation(ParameterExpression parameter, PropertyOperation property)
    {
        if (property.Name == "City")
        {
            var addresses = Expression.Property(parameter, "Addresses");
            var addressParam = Expression.Parameter(typeof(Address), "y");
            var condition = EvaluateBinaryOperation(
                property,
                Expression.Property(addressParam, property.Name),
                Expression.Constant(property.Value));
            return Expression.Call(
                typeof(Enumerable),
                "Any",
                [typeof(Address)],
                addresses,
                Expression.Lambda<Func<Address, bool>>(condition, addressParam)
            );
        }
        return base.EvaluatePropertyOperation(parameter, property);
    }
}

public class OperatorFilterTests(ITestOutputHelper output) : BaseEFTest(output)
{
    private void ApplyFilterAndCheckCount(string filterString, int expectedCount)
    {
        var options = new FilterOptions<Product>()
                      .WithNullValue("NULL_VALUE")
                      .WithStringEmpty("EMPTY_VALUE")
                      .AddProperty<string>("City")
                      .AddOperation("LIKE", (x,y) => new LikeOperation(x,y));

        var parser = new PolishNotationFilterParser(options);
        var filter = parser.Parse(filterString);
        using var context = GetContext();
        context.Products.ApplyFilter(filter, new CustomProductFilterEvaluator()).Count().Should().Be(expectedCount);
    }


    [Theory]
    [InlineData("eq City Brussels",3)]
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
    [InlineData("not eq Id 10",25)]
    [InlineData("not eq Name Smartwatch",25)]
    [InlineData("not eq Price 149.99",24)]
    [InlineData("not eq IsOutOfStock false",13)]
    [InlineData("not eq CreationDate 2023-12-30",23)]
    public void TestNotOperatorFilter(string filterString, int expectedCount)
    {
        ApplyFilterAndCheckCount(filterString, expectedCount);
    }

    [Theory]
    [InlineData("gt Id 10",16)]
    [InlineData("gt Price 199.99",14)]
    [InlineData("gt CreationDate 2023-12-15",15)]
    public void TestGreaterThanOperatorFilter(string filterString, int expectedCount)
    {
        ApplyFilterAndCheckCount(filterString, expectedCount);
    }

    [Theory]
    [InlineData("gte Id 10",17)]
    [InlineData("gte Price 199.99",17)]
    [InlineData("gte CreationDate 2023-12-30",15)]
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
    [InlineData("Contains Name a",18)]
    public void TestContainsOperatorFilter(string filterString, int expectedCount)
    {
        ApplyFilterAndCheckCount(filterString, expectedCount);
    }

    [Theory]
    [InlineData("Like Name a%",1)]
    public void TestLikeOperatorFilter(string filterString, int expectedCount)
    {
        ApplyFilterAndCheckCount(filterString, expectedCount);
    }

    [Theory]
    [InlineData("Eq Name NULL_VALUE",2)]
    public void TestNullFilter(string filterString, int expectedCount)
    {
        ApplyFilterAndCheckCount(filterString, expectedCount);
    }

    [Theory]
    [InlineData("Eq Name EMPTY_VALUE",1)]
    public void TestEmptyValueFilter(string filterString, int expectedCount)
    {
        ApplyFilterAndCheckCount(filterString, expectedCount);
    }
}