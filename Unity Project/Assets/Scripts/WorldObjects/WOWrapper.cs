using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WOWrapper : MonoBehaviour
{
    public WorldObject WO { get; set; }

    public WOWrapper()
    {
    }

    public W SetTo<W>(W item) where W : WorldObject
    {
        WO = item;
        WO.Instance = this;
        return (W)WO;
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
        WO = null;
    }

    public static implicit operator WorldObject (WOWrapper inst)
    {
        return inst.WO;
    }
}
