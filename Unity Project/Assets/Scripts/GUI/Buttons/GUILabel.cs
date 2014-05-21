using System;
using UnityEngine;

public class GUILabel : UIButton
{
    public UILabel label;
    public string Text
    {
        get
        {
            if (label == null)
            {
                label = new UILabel();
            }
            return label.text;
        }
        set
        {
            if (label == null)
            {
                label = new UILabel();
            }
            label.text = value;
        }
    }

    public UIDragPanelContents UIDragPanel;

    public override void OnHover(bool isOver)
    {
        //base.OnHover(isOver);
    }

    public override void OnPress(bool isPressed)
    {
        //base.OnPress(isPressed);
    }

    void OnClick()
    {
        BigBoss.Gooey.DisplayChatGrid = !BigBoss.Gooey.DisplayChatGrid;
        if (BigBoss.Gooey.DisplayChatGrid)
        {
            BigBoss.Gooey.ChatGrid.GridPanel.clipRange = new Vector4(50, -400, 500, 800);
            BigBoss.Gooey.ChatGrid.displayLimit = 20;
            BigBoss.Gooey.ChatGrid.repositionNow = true;
        }
        else
        {
            BigBoss.Gooey.ChatGrid.GridPanel.clipRange = new Vector4(50, -75, 500, 140);
            BigBoss.Gooey.ChatGrid.displayLimit = 5;
            BigBoss.Gooey.ChatGrid.repositionNow = true;
        }
    }

    public void Fix()
    {
        this.label.MakePixelPerfect();
        this.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        this.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
    }
}