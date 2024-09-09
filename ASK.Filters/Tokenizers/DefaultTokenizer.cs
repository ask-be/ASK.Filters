namespace ASK.Filters.Tokenizers;

public class DefaultTokenizer(char separator = ' ') : ITokenizer
{
    public Stack<string> Tokenize(string input)
    {
        return new Stack<string>(input.Split(separator).Reverse());
    }
}