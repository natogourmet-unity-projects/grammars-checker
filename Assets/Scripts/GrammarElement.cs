public class GrammarElement
{
    private bool isNonTerminal;
    private string symbol;

    public GrammarElement(bool isNonTerminal, string symbol)
    {
        this.isNonTerminal = isNonTerminal;
        this.symbol = symbol;
    }

    public bool IsNonTerminal()
    {
        return isNonTerminal;
    }

    public string GetSymbol()
    {
        return symbol;
    }
}