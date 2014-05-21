using System;
using System.Collections.Generic;
using UnityEngine;

public class TextGrid : ScrollingGrid
{
    public List<GUIButton> labels = new List<GUIButton>();
    public int displayLimit = 5;
    public int maxLabels;
    public bool repositionNow = false;

    void Awake()
    {
        GridPanel = GetComponentInChildren<UIPanel>();
        DragPanel = GetComponentInChildren<UIDraggablePanel>();
        ScrollPanel = GetComponentInChildren<UIScrollBar>();

        Grid = GetComponentInChildren<KGrid>();
    }

    void Start()
    {
        this.gameObject.SetActive(InitiallyActive);
    }

    void Update()
    {
        if (repositionNow)
        {
            repositionNow = false;
            Reposition();
        }
    }

    internal void AddLabel(GUIButton label)
    {
        label.gameObject.SetActive(false);
        labels.Add(label);
        label.useHover = false;
        label.usePress = false;
        if (labels.Count > maxLabels)
        {
            Destroy(labels[0].gameObject);
            labels.RemoveAt(0);
        }
        Reposition();
    }

    public override void Reposition()
    {
        foreach (GUIButton g in labels)
        {
            g.gameObject.SetActive(false);
        }
        Grid.ClearList();
        int counter = 0;
        for (int i = labels.Count; i--> 0; )
        {
            if (counter > displayLimit) break;
            counter++;
            labels[i].gameObject.SetActive(true);
            Grid.AddButton(labels[i]);
            labels[i].Fix();
        }
        GridPanel.transform.localPosition = Vector3.zero;
        Grid.Reposition();
        GridPanel.transform.localPosition = Vector3.zero;
    }
}
