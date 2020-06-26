using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionButton : MonoBehaviour
{
    public void ProductionClick()
    {
        GrammarCreator.instance.ProductionClick(this.gameObject);
    }
}
