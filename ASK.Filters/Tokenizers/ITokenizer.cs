namespace ASK.Filters.Tokenizers;

public interface ITokenizer
{
    /// <summary>
    /// Return the expression as a Stack of tokens ordered as Polish Notation
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Stack<string> Tokenize(string input);
}