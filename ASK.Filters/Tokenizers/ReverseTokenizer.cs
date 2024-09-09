namespace ASK.Filters.Tokenizers;

public class ReverseTokenizer(char separator = ' ') : ITokenizer
{
    public Stack<string> Tokenize(string input)
    {
        return new Stack<string>(input.Split(separator));
    }
}