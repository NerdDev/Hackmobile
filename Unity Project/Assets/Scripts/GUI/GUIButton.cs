using System;
using UnityEngine;


public class GUIButton : UIButton
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

    internal System.Object refObject;

    private Action _OnClick;
    public Action OnSingleClick
    {
        get { if (_OnClick == null) _OnClick = new Action(() => { }); return _OnClick; }
        set { _OnClick = value; }
    }
    void OnClick()
    {
        OnSingleClick();
    }

    private Action _OnDoubleClick;
    public Action On2Clicks
    {
        get { if (_OnDoubleClick == null) _OnDoubleClick = new Action(() => { }); return _OnDoubleClick; }
        set { _OnDoubleClick = value; }
    }
    void OnDoubleClick()
    {
        On2Clicks();
    }

    private Action<bool> _OnPress;
    public Action<bool> OnButtonPress
    {
        get { if (_OnPress == null) _OnPress = new Action<bool>((b) => { }); return _OnPress; }
        set { _OnPress = value; }
    }
    void OnPress(bool onPress)
    {
        OnButtonPress(onPress);
    }
}