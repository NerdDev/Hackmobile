using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WOInstance<W> : MonoBehaviour where W : WorldObject, new()
{
    public W WO { get; set; }

    public WOInstance()
    {
    }

    public void SetTo(W item)
    {
        WO = item.Copy();
        WO.GO = this.gameObject;
    }

    public static implicit operator W(WOInstance<W> inst)
    {
        return inst.WO;
    }
}
