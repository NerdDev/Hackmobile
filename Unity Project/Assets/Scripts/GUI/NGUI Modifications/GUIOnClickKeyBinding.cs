//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Very basic script that will activate or deactivate an object (and all of its children) when clicked.
/// </summary>

[AddComponentMenu("Game/UI/Button OnClick Key Binding")]
public class GUIOnClickKeyBinding : MonoBehaviour
{
    public KeyCode keyCode = KeyCode.None;

    void Update()
    {
        if (!UICamera.inputHasFocus)
        {
            if (keyCode == KeyCode.None) return;

            if (Input.GetKeyUp(keyCode))
            {
                SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}