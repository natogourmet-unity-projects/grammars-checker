using System.Collections.Generic;

public class GrammarProduction
{
    private GrammarElement leftSide;
    private List<GrammarElement> rightSide;
    
    public GrammarProduction(GrammarElement leftSide, List<GrammarElement> rightSide)
    {
        this.leftSide = leftSide;
        this.rightSide = rightSide;
    }

    public GrammarElement GetLeftSide()
    {
        return leftSide;
    }

    public void SetLeftSide(GrammarElement leftSide)
    {
        this.leftSide = leftSide;
    }

    public List<GrammarElement> GetRightSide()
    {
        return rightSide;
    }

    public void SetRightSide(List<GrammarElement> rightSide)
    {
        this.rightSide = rightSide;
    }
}