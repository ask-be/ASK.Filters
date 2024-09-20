using FluentAssertions;

namespace ASK.Filters.Tests;

public class FilterOptionsTests
{
    [Fact]
    public void EncureCkearOptionsDoesNotDeleteProperties()
    {
        var o = new FilterOptions([new FilterProperty<string>("name")]);
        o.FilterProperties.Count.Should().Be(1);

        o.ClearOperations();
        o.FilterProperties.Count.Should().Be(1);
    }
}