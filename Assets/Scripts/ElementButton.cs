using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementButton : MonoBehaviour
{
    public void ElementClick()
    {
        GrammarCreator.instance.ElementClick(this.gameObject);
    }
}
