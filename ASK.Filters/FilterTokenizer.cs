namespace ASK.Filters;

public static class FilterTokenizer
{
    public static Stack<string> Tokenize(string input) => new (input.Split(' ').Reverse());
}