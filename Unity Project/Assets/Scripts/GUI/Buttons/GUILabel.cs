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

    protected override void OnHover(bool isOver)
    {
    }

    protected override void OnPress(bool isPressed)
    {
    }

    public void Fix()
    {
        this.label.MakePixelPerfect();
        this.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        this.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
    }
}