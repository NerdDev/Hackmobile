using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WOInstance : MonoBehaviour
{
    public WorldObject WO { get; set; }

    public WOInstance()
    {
    }

    public W SetTo<W>(W item) where W : WorldObject
    {
        WO = item.Copy();
        WO.GO = this.gameObject;
        return (W)WO;
    }

    public static implicit operator WorldObject (WOInstance inst)
    {
        return inst.WO;
    }
}
