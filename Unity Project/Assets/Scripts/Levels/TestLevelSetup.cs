using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class TestLevelSetup : MonoBehaviour
{
    public abstract LevelLayout Create();
    public virtual int StartX { get { return 0; } }
    public virtual int StartY { get { return 0; } }
    public abstract void Spawn(Level level);
}