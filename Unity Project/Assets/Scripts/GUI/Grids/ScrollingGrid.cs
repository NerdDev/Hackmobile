using System;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingGrid : GUIElement
{
    public UIPanel GridPanel;
    public UIScrollBar ScrollPanel;
    public KGrid Grid;
    public bool InitiallyActive = false;

    void Awake()
    {
        GridPanel = GetComponentInChildren<UIPanel>();
        ScrollPanel = GetComponentInChildren<UIScrollBar>();

        Grid = GetComponentInChildren<KGrid>();
    }

    void Start()
    {
        this.gameObject.SetActive(InitiallyActive);
    }

    public override void Clear()
    {
        this.Grid.Clear();
    }

    public virtual void Reposition()
    {
        this.Grid.Reposition();
    }

    public void AddButton(GUIButton button)
    {
        Grid.AddButton(button);
        button.Fix();
    }
}
