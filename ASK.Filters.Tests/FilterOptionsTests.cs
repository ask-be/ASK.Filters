using ASK.Filters.Operations;
using FluentAssertions;

namespace ASK.Filters.Tests;

public class FilterOptionsTests
{
    [Fact]
    public void EnsureClearOptionsDoesNotDeleteProperties()
    {
        // Arrange
        var o = new FilterOptions([new FilterProperty<string>("name")]);

        // Act
        o.ClearOperations();

        // Assert
        o.FilterProperties.Count.Should().Be(1);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenPropertiesIsNull()
    {
        // Arrange
        IEnumerable<FilterProperty> properties = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new FilterOptions(properties));
    }

    [Fact]
    public void AddOperation_ShouldFail_WhenOperationNameIsNull()
    {
        // Arrange
        var properties = new List<FilterProperty> { new FilterProperty("Name", typeof(string)) };
        var filterOptions = new FilterOptions(properties);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => filterOptions.AddOperation(null, (x, y) => new AndOperation(x, y)));
    }


    [Fact]
    public void WithNullValue_ShouldFail_WhenNullValueIsNull()
    {
        // Arrange
        var properties = new List<FilterProperty> { new FilterProperty("Name", typeof(string)) };
        var filterOptions = new FilterOptions(properties);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => filterOptions.WithNullValue(null));
    }

    [Fact]
    public void AddConverter_ShouldFail_WhenConverterIsNull()
    {
        // Arrange
        var properties = new List<FilterProperty> { new FilterProperty("Name", typeof(string)) };
        var filterOptions = new FilterOptions(properties);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => filterOptions.AddConverter<string>(null));
    }

    [Fact]
    public void ClearOperations_ShouldFail_WhenOperationsAreNotCleared()
    {
        // Arrange
        var properties = new List<FilterProperty> { new FilterProperty("Name", typeof(string)) };
        var filterOptions = new FilterOptions(properties);
        filterOptions.AddOperation("CUSTOM_AND", (x, y) => new AndOperation(x, y));

        // Act
        filterOptions.ClearOperations();

        // Assert
        Assert.False(filterOptions.BinaryOperations.ContainsKey("CUSTOM_AND"));
    }

    [Fact]
    public void WithNullValue_ShouldFail_WhenNullValueIsEmptyString()
    {
        // Arrange
        var properties = new List<FilterProperty> { new FilterProperty("Name", typeof(string)) };
        var filterOptions = new FilterOptions(properties);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => filterOptions.WithNullValue(string.Empty));
    }
}