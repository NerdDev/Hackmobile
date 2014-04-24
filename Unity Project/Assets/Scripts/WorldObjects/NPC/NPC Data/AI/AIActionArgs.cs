using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AIActionArgs
{
    private AICore core;
    public NPC Self { get { return core.NPC; } }
    private NPC _target;
    public NPC Target
    {
        get { return _target; }
        set
        {
            _target = value;
            if (_target != null)
            {
                TargetSpace = _target.GridSpace;
            }
        }
    }
    public GridSpace TargetSpace;
    public AIState CurrentState { get { return core.CurrentState; } set { core.CurrentState = value; } }
    public AIDecision LastDecision { get { return core.LastDecision; } }
    public AIDecision CurrentDecision;
    public bool Continuing { get { return System.Object.ReferenceEquals(LastDecision, CurrentDecision); } }
    public Level Level { get { return BigBoss.Levels.Level; } }
    public System.Random Random { get { return core.Random; } }

    public AIActionArgs(AICore core)
    {
        this.core = core;
    }

    public void MoveTo(int x, int y)
    {
        core.Move(x, y);
    }

    public void MoveTo(GridSpace space)
    {
        core.Move(space);
    }

    public void MoveTo(WorldObject obj)
    {
        MoveTo(obj.GridSpace);
    }
}
