using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrammarCreator : MonoBehaviour
{
    public GameObject NTContainer;
    public GameObject TContainer;
    public GameObject productionsContainer;
    public GameObject elementButton;
    public GameObject productionButton;
    public Text currentProductionText;
    public GameObject errorPanel;
    public Text errorText;
    public GameObject finalPanel;
    public Text finalText;

    Dictionary<GameObject, GrammarElement> elements;
    Dictionary<GameObject, GrammarProduction> productions;
    Grammar grammar;

    GrammarProduction currentProduction;
    bool newProduction = true;
    GameObject currentProductionButton;

    #region Singleton
    public static GrammarCreator instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    void Start()
    {
        elements = new Dictionary<GameObject, GrammarElement>();
        this.productions = new Dictionary<GameObject, GrammarProduction>();

        currentProduction = new GrammarProduction(null, new List<GrammarElement>());


    }

    public bool VerifyExistence(string symbol)
    {
        if (symbol == "") return true;
        foreach (KeyValuePair<GameObject, GrammarElement> element in elements)
        {
            if (element.Value.GetSymbol().Equals(symbol)) return true;
        }
        return false;
    }

    public void NewNonTerminal(Text text)
    {
        string symbol = text.text.ToUpper();
        if (!VerifyExistence(symbol))
        {
            GrammarElement newNT = new GrammarElement(true, symbol);
            GameObject newNTButton = Instantiate(elementButton, NTContainer.transform);
            newNTButton.transform.GetChild(0).GetComponent<Text>().text = "<" + symbol + ">";
            elements.Add(newNTButton, newNT);
        }
    }

    public void NewTerminal(Text text)
    {
        string symbol = text.text.ToLower();
        if (!VerifyExistence(symbol))
        {
            GrammarElement newNT = new GrammarElement(false, symbol);
            GameObject newNTButton = Instantiate(elementButton, TContainer.transform);
            newNTButton.transform.GetChild(0).GetComponent<Text>().text = symbol;
            elements.Add(newNTButton, newNT);
        }
    }

    public string GetProductionString(GrammarProduction production)
    {
        string productionString = "<";
        if (production.GetLeftSide() != null) productionString += production.GetLeftSide().GetSymbol();
        productionString += "> → ";
        if (production.GetRightSide().Count == 0) productionString += "λ";
        else
        {
            foreach (GrammarElement element in production.GetRightSide())
            {
                if (element.IsNonTerminal()) productionString += "<" + element.GetSymbol() + ">";
                else
                {
                    productionString += element.GetSymbol();
                }
            }
        }
        return productionString;
    }

    public void ElementClick(GameObject elementButton)
    {
        GrammarElement currentElement = elements[elementButton];

        if (currentProduction == null)
        {
            currentProduction = new GrammarProduction(null, new List<GrammarElement>());
        }

        if (currentProduction.GetLeftSide() == null)
        {
            if (currentElement.IsNonTerminal())
            {
                currentProduction.SetLeftSide(currentElement);
            }
        }
        else
        {
            currentProduction.GetRightSide().Add(currentElement);
        }

        if (!newProduction) currentProductionButton.transform.parent.GetChild(1).GetComponentInChildren<Text>().text = GetProductionString(currentProduction);
        currentProductionText.text = GetProductionString(currentProduction);
    }

    public void Delete()
    {
        if (currentProduction != null)
        {
            if (currentProduction.GetRightSide().Count > 0)
            {
                currentProduction.GetRightSide().RemoveAt(currentProduction.GetRightSide().Count - 1);
            }
            else
            {
                currentProduction.SetLeftSide(null);
                if (!newProduction)
                {
                    productions.Remove(currentProductionButton);
                    Destroy(currentProductionButton.transform.parent.gameObject);
                    currentProductionButton = null;
                    newProduction = true;
                    ReorderElements();
                }
            }
            if (!newProduction) currentProductionButton.transform.parent.GetChild(1).GetComponentInChildren<Text>().text = GetProductionString(currentProduction);
            currentProductionText.text = GetProductionString(currentProduction);
        }
        
    }

    public void ReorderElements()
    {
        int c = 1;
        foreach (KeyValuePair<GameObject, GrammarProduction> production in productions)
        {
            production.Key.transform.parent.GetChild(0).GetComponentInChildren<Text>().text = c.ToString();
            c++;
        }
    }

    public void AddProduction()
    {
        if (newProduction && currentProduction.GetLeftSide() != null)
        {
            if (!EvaluateProductionExistence(currentProduction))
            {
                GameObject newProductionButton = Instantiate(productionButton, productionsContainer.transform);
                newProductionButton.transform.GetChild(0).GetComponentInChildren<Text>().text = (productions.Count + 1).ToString();
                newProductionButton.transform.GetChild(1).GetComponentInChildren<Text>().text = GetProductionString(currentProduction);
                productions.Add(newProductionButton.transform.GetChild(1).gameObject, currentProduction);
            }
        }
        else if (!newProduction)
        {
            newProduction = true;
        }
        currentProduction = new GrammarProduction(null, new List<GrammarElement>());
        currentProductionText.text = GetProductionString(currentProduction);
    }

    public bool EvaluateProductionExistence(GrammarProduction production)
    {
        foreach (KeyValuePair<GameObject, GrammarProduction> p in productions)
        {
            if (p.Value.GetLeftSide() == production.GetLeftSide() && (p.Value.GetRightSide().Count == production.GetRightSide().Count))
            {
                int c = 0;
                bool different = false;
                List<GrammarElement> rightSide1 = p.Value.GetRightSide();
                List<GrammarElement> rightSide2 = production.GetRightSide();
                while (c < rightSide1.Count)
                {
                    if (rightSide1[c] != rightSide2[c])
                    {
                        different = true;
                        break;
                    }
                    c++;
                }
                if (!different) return true;
            }
        }
        return false;
    }

    public void ProductionClick(GameObject productionButton)
    {
        currentProductionButton = productionButton;
        currentProduction = productions[currentProductionButton];
        currentProductionText.text = GetProductionString(currentProduction);
        newProduction = false;
    }

    public void Evaluate()
    {
        if (this.productions.Count < 1)
        {
            ErrorMessage("Ninguna Producción");
            return;
        }

        List<GrammarElement> nonTerminals = new List<GrammarElement>();
        foreach (KeyValuePair<GameObject, GrammarProduction> production in this.productions)
        {
            GrammarElement currentElement = production.Value.GetLeftSide();
            if (!nonTerminals.Contains(currentElement)) nonTerminals.Add(currentElement);
        }

        List<GrammarElement> currentList;
        foreach (KeyValuePair<GameObject, GrammarProduction> production in this.productions)
        {
            currentList = production.Value.GetRightSide();
            foreach (GrammarElement element in currentList)
            {
                if (element.IsNonTerminal() && !nonTerminals.Contains(element))
                {
                    ErrorMessage("No existe una producción que defina a <" + element.GetSymbol() + ">");
                    return;
                }
            }
        }

        List<GrammarProduction> productions = new List<GrammarProduction>();
        foreach (KeyValuePair<GameObject, GrammarProduction> production in this.productions)
        {
            productions.Add(production.Value);
        }

        SendGrammar(nonTerminals, productions);
    }

    public void SendGrammar(List<GrammarElement> nt, List<GrammarProduction> productions)
    {
        grammar = new Grammar(productions, nt);
        Print();
    }

    public void ErrorMessage(string text)
    {
        errorPanel.SetActive(true);
        errorText.text = text;
    }

    public void Print()
    {
        string Final = "";

        List<bool> types = grammar.GetGrammarTypes();
        if (types[0]) Final += "Is S Gramar\n" ;
        if (types[1]) Final += "Is Q Gramar\n";
        if (types[2]) Final += "Is SpecialForm Gramar\n";
        if (types[3]) Final += "Is RightLinear Gramar\n";
        if (types[4]) Final += "Is LL1 Gramar\n";

        Final += "\n";

        List<string> firsts = grammar.GetFirstOfEachNT();
        Final += "Primeros de NT\n";
        foreach (string item in firsts)
        {
            Final += item + "\n";
        }

        Final += "\n";

        firsts = grammar.GetFirstOfEachProduction();
        Final += "Primeros de Producción\n";
        foreach (string item in firsts)
        {
            Final += item + "\n";
        }

        Final += "\n";

        firsts = grammar.GetNextOfEachNT();
        Final += "Siguientes de NT\n";
        foreach (string item in firsts)
        {
            Final += item + "\n";
        }

        Final += "\n";

        firsts = grammar.GetSelectionOfEachProduction();
        Final += "Selección de Cada Producción\n";
        for(int i = 0; i < firsts.Count; i++)
        {
            Final += i+1 + ") = {" + firsts[i] + "}\n";
        }

        finalPanel.SetActive(true);
        finalText.text = Final;
    }
}
