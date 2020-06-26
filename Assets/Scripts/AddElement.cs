using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddElement : MonoBehaviour
{
    public Text symbol;

    public void AddNewElement(bool isNonTerminal)
    {
        if (isNonTerminal)
        {
            GrammarCreator.instance.NewNonTerminal(symbol);
        }
        else
        {
            GrammarCreator.instance.NewTerminal(symbol);
        }
    }
}
