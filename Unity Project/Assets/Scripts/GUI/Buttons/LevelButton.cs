using System;
using UnityEngine;

public class LevelButton : GUIButton
{
    public bool up;

    public override void Initialize()
    {
        base.Initialize();
        OnSingleClick = new Action(() =>
        {
            BigBoss.Levels.SetCurLevel(up);
        });
    }
}