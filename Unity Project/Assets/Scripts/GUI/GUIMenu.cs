using System;
using System.Collections.Generic;
using UnityEngine;

public class GUIMenu : MonoBehaviour
{
    public bool isActive = false;
    public bool displayMenu = false;
    public List<GUIElement> Elements = new List<GUIElement>();

    public virtual void Display()
    {
        isActive = true;
        this.gameObject.SetActive(true);
        foreach (GUIElement element in Elements)
        {
            element.parent = this;
            element.Initialize();
        }
    }

    public virtual void Open()
    {
        Display();
    }

    public virtual void Close()
    {
        Clear();
    }

    public void Clear()
    {
        isActive = false;
        this.gameObject.SetActive(false);
        foreach (GUIElement element in Elements)
        {
            element.Clear();
        }
    }

    public T Type<T>() where T : GUIMenu
    {
        T ret = this is T ? (T) this : null;
        return ret;
    }
}