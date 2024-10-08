using System.Text;

namespace ASK.Filters;

public static class Tokenizer
{
    private const char Quote = '\'';

    public static IEnumerable<string> Tokenize(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            yield break; // Return an empty collection for null or empty input
        }

        var currentToken = new StringBuilder();
        bool betweenTokens = true;
        var insideQuotes = false;

        for (var i = 0; i < input.Length; i++)
        {
            var currentChar = input[i];

            if (currentChar == Quote)
            {
                if (insideQuotes)
                {
                    if(input.Length > i+1 && input[i + 1] == Quote)
                    {
                        currentToken.Append(Quote);
                        i++;
                    }
                    else
                    {
                        yield return currentToken.ToString();
                        currentToken.Clear();
                        betweenTokens = true;
                        insideQuotes = false;
                    }
                }
                else
                {
                    insideQuotes = true;
                    betweenTokens = false;
                }
            }
            else if (char.IsWhiteSpace(currentChar) && !insideQuotes)
            {
                if(betweenTokens)
                    continue;

                yield return currentToken.ToString();
                currentToken.Clear();
                betweenTokens = true;
            }
            else
            {
                betweenTokens = false;
                currentToken.Append(currentChar);
            }
        }

        // Return the last token if any
        if (currentToken.Length > 0)
        {
            if(insideQuotes)
                throw new FormatException("Unclosed quote");

            yield return currentToken.ToString();
        }
    }

}