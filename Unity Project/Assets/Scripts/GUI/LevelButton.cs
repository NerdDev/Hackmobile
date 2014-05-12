using System;
using UnityEngine;

public class LevelButton : GUIButton
{
    public int LevelToGo;

    public virtual void Start()
    {
        Text = "Change Level";
        OnSingleClick = new Action(() =>
        {
            BigBoss.Levels.SetCurLevel(LevelToGo);
        });
    }
}