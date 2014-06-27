using System;
using UnityEngine;

public class DebugButton : GUIButton
{
    public DebugConsole console = null;

    void Start()
    {
        OnSingleClick = new Action(() =>
        {
            if (console != null)
            {
                console.visible = !console.visible;
            }
            else
            {
                GameObject go = GameObject.Find("DebugConsole");
                if (go != null)
                {
                    DebugConsole debug = go.GetComponent<DebugConsole>();
                    if (debug != null)
                    {
                        console = debug;
                        console.visible = !console.visible;
                    }
                }
            }
        });
    }
}