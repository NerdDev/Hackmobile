﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingGrid : MonoBehaviour
{
    public UIPanel GridPanel;
    public UIDraggablePanel DragPanel;
    public UIScrollBar ScrollPanel;
    public KGrid Grid;

    void Awake()
    {
        GridPanel = GetComponentInChildren<UIPanel>();
        DragPanel = GetComponentInChildren<UIDraggablePanel>();
        ScrollPanel = GetComponentInChildren<UIScrollBar>();

        Grid = GetComponentInChildren<KGrid>();
    }

    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void Clear()
    {
        this.Grid.Clear();
    }

    public void Reposition()
    {
        this.Grid.Reposition();
    }

    public void ResetPosition()
    {
        this.DragPanel.ResetPosition();
    }

    public void AddButton(GUIButton button)
    {
        Grid.AddButton(button);
        button.Fix();
    }
}
