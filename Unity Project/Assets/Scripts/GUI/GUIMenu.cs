using System;
using System.Collections.Generic;
using UnityEngine;

public class GUIMenu : MonoBehaviour
{
    public bool displayMenu = false;
    public List<GUIElement> Elements = new List<GUIElement>();

    public void Display()
    {
        this.gameObject.SetActive(true);
        foreach (GUIElement element in Elements)
        {
            element.parent = this;
            element.Initialize();
        }
    }

    public void Clear()
    {
        this.gameObject.SetActive(false);
        foreach (GUIElement element in Elements)
        {
            element.Clear();
        }
    }
}