using System;
using UnityEngine;

public class GUIElement : MonoBehaviour
{
    public GUIMenu parent;

    public virtual void Initialize()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void Clear()
    {
        this.gameObject.SetActive(false);
    }
}