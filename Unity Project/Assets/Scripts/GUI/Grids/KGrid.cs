//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// All children added to the game object with this script will be repositioned to be on a grid of specified dimensions.
/// If you want the cells to automatically set their scale based on the dimensions of their content, take a look at UITable.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Grid")]
public class KGrid : GUIElement
{
    public enum Arrangement
    {
        Horizontal,
        Vertical,
    }

    public Arrangement arrangement = Arrangement.Horizontal;
    public int maxPerLine = 0;
    public float cellWidth = 200f;
    public float cellHeight = 200f;
    public bool repositionNow = false;
    public bool sorted = false;
    public bool hideInactive = true;

    public bool ReverseList = false;

    public bool useList = true;
    public List<Transform> gridObjects = new List<Transform>();

    bool mStarted = false;

    void Start()
    {
        mStarted = true;
        Reposition();
    }

    void Update()
    {
        if (repositionNow)
        {
            repositionNow = false;
            Reposition();
        }
    }

    static public int SortByName(Transform a, Transform b) { return string.Compare(a.name, b.name); }

    /// <summary>
    /// Recalculate the position of all elements within the grid, sorting them alphabetically if necessary.
    /// </summary>

    public void Reposition()
    {
        if (!mStarted)
        {
            repositionNow = true;
            return;
        }

        int x = 0;
        int y = 0;

        if (ReverseList)
        {
            for (int i = gridObjects.Count; i-- > 0; )
            {
                if (!NGUITools.GetActive(gridObjects[i].gameObject) && hideInactive) continue;

                float depth = gridObjects[i].localPosition.z;
                gridObjects[i].localPosition = (arrangement == Arrangement.Horizontal) ?
                    new Vector3(cellWidth * x, -cellHeight * y, depth) :
                    new Vector3(cellWidth * y, -cellHeight * x, depth);

                if (++x >= maxPerLine && maxPerLine > 0)
                {
                    x = 0;
                    ++y;
                }
            }
        }
        else
        {
            foreach (Transform t in gridObjects)
            {
                if (!NGUITools.GetActive(t.gameObject) && hideInactive) continue;

                float depth = t.localPosition.z;
                t.localPosition = (arrangement == Arrangement.Horizontal) ?
                    new Vector3(cellWidth * x, -cellHeight * y, depth) :
                    new Vector3(cellWidth * y, -cellHeight * x, depth);

                if (++x >= maxPerLine && maxPerLine > 0)
                {
                    x = 0;
                    ++y;
                }
            }
        }

        UIDraggablePanel drag = NGUITools.FindInParents<UIDraggablePanel>(gameObject);
        if (drag != null) drag.UpdateScrollbars(true);
    }

    public void AddButton(GUIButton button)
    {
        gridObjects.Add(button.transform);
        button.transform.parent = this.transform;
    }

    public void Clear()
    {
        foreach (Transform t in gridObjects)
        {
            Destroy(t.gameObject);
        }
        gridObjects.Clear();
    }

    public void ClearList()
    {
        gridObjects.Clear();
    }
}