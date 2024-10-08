using FluentAssertions;

namespace ASK.Filters.Tests;

public class TokenizerTests
{
    [Theory]
    [InlineData("eq Name John", new []{"eq","Name","John"})]
    [InlineData("eq Name 'John'", new []{"eq","Name","John"})]
    [InlineData("eq Name 'John Doe'", new []{"eq","Name","John Doe"})]
    [InlineData("eq Name 'John''Doe'", new []{"eq","Name","John'Doe"})]
    [InlineData("eq Name 'John ''Doe'", new []{"eq","Name","John 'Doe"})]
    [InlineData("eq Name 'John '' Doe'", new []{"eq","Name","John ' Doe"})]
    [InlineData("eq Name 'John'' Doe'", new []{"eq","Name","John' Doe"})]
    [InlineData("first 'John' and '' or  'Doe'", new []{"first","John","and","","or", "Doe"})]
    [InlineData("     ", new string[0])]
    [InlineData("  Hello   ", new []{"Hello"})]
    [InlineData("  'Hello'   ", new []{"Hello"})]
    [InlineData("'Hello'", new []{"Hello"})]
    public void Tokenize(string input, string[] expected)
    {
        var tokens = Tokenizer.Tokenize(input);

        tokens.Should().BeEquivalentTo(expected);
    }
}