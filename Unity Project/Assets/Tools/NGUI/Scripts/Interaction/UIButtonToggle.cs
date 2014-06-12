//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Very basic script that will activate or deactivate an object (and all of its children) when clicked.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Button Toggle")]
public class UIButtonToggle : MonoBehaviour
{
    public List<GameObject> targets;
    public bool state = true;

    void OnClick()
    {
        if (targets == null) return;
        if (targets[0] == null) return;
        state = targets[0].activeSelf;

        foreach (GameObject target in targets)
        {
            if (target != null) NGUITools.SetActive(target, !state);
        }
    }
}