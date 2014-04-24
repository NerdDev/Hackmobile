using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WOWrapper : MonoBehaviour
{
    public WorldObject WO;
    public int ID;

    public WOWrapper()
    {
    }

    public W SetTo<W>(W item) where W : WorldObject
    {
        WO = item;
        WO.Instance = this;
        ID = item.ID;
        return (W)WO;
    }

    public void Destroy()
    {
        WO = null;
        this.gameObject.SetActive(false);
        GameObject.Destroy(this.gameObject);
    }

    public static implicit operator WorldObject (WOWrapper inst)
    {
        return inst.WO;
    }

    void OnMouseDown()
    {
        WO.OnClick();
    }
}
