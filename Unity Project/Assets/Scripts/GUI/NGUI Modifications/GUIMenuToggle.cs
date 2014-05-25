//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Very basic script that will activate or deactivate an object (and all of its children) when clicked.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Menu Toggle")]
public class GUIMenuToggle : MonoBehaviour
{
    public List<GUIMenu> targets;
    public bool state = true;
    public bool useKeyCode = true;
    public KeyCode keyCode = KeyCode.None;

    void Update()
    {
        if (useKeyCode)
        {
            if (!UICamera.inputHasFocus)
            {
                if (keyCode == KeyCode.None) return;

                if (Input.GetKeyUp(keyCode))
                {
                    OnClick();
                }
            }
        }
    }

    void OnClick()
    {
        if (targets == null) return;
        if (targets[0] == null) return;
        state = targets[0].isActive;

        foreach (GUIMenu target in targets)
        {
            if (state)
            {
                target.Close();
            }
            else
            {
                target.Open();
            }
        }
    }
}