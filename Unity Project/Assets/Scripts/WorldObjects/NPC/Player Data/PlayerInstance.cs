using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerInstance : NPCInstance, IManager
{
    public bool Initialized { get; set; }
    public void Initialize()
    {
        this.SetTo(new Player());
        WO.Init();
        Rendering(false);
    }

    public void Rendering(bool render)
    {
        Renderer[] mRenderers = GetComponentsInChildren<Renderer>();
        for (int i = 0, imax = mRenderers.Length; i < imax; ++i)
        {
            Renderer ren = mRenderers[i];

            if (ren)
            {
                ren.enabled = render;
            }
        }
    }

    public BoneStructure bones;
}
