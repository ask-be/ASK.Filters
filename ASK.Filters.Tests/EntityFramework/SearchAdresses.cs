using System.Linq.Expressions;
using System.Reflection;
using ASK.Filters.Operations;
using FluentAssertions;
using Xunit.Abstractions;

namespace ASK.Filters.Tests.EntityFramework;


public class CustomAddressFilterEvaluator : FilterEvaluator<Address>
{
    private static readonly MethodInfo ToUpperMethod =  typeof(string).GetMethod("ToUpper", Type.EmptyTypes)!;

    public CustomAddressFilterEvaluator()
    {
        AddOperationEvaluator<LikeOperation>(new LikeOperationEvaluator());
    }

    protected override Expression EvaluatePropertyOperation(ParameterExpression parameter, PropertyOperation property)
    {
        if (property is {Name: "Address", Value: not null})
        {
            var parts = property.Value.ToString()!.ToUpper().Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Distinct();

            Expression expression = Expression.Constant(true);

            foreach (var part in parts)
            {
                var constant = Expression.Constant(part);
                expression = Expression.AndAlso(expression, Expression.OrElse(
                    Expression.OrElse(
                        EvaluateBinaryOperation(property, Expression.Call(Expression.Property(parameter, nameof(Address.City)), ToUpperMethod), constant),
                        EvaluateBinaryOperation(property, Expression.Call(Expression.Property(parameter, nameof(Address.Country)),ToUpperMethod), constant)
                    ),
                    EvaluateBinaryOperation(property, Expression.Call(Expression.Property(parameter, nameof(Address.State)),ToUpperMethod), constant)
                ));
            }

            return expression;
        }
        return base.EvaluatePropertyOperation(parameter, property);
    }
}

public class SearchAdresses(ITestOutputHelper output) : BaseEFTest(output)
{
    [Fact]
    public void Test()
    {
        var options = new FilterOptions<Address>().AddProperty<string>("Address");

        var parser = new PolishNotationFilterParser(options);
        var filter = parser.Parse("Contains Address 'Bru Be s'");
        using var context = GetContext();
        context.Addresses.ApplyFilter(filter, new CustomAddressFilterEvaluator()).Count().Should().Be(1);
    }
}