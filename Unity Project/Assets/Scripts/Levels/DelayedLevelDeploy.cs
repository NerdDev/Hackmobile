using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DelayedLevelDeploy
{
    public int Counter;
    public GridSpace space;
    public List<GridDeploy> DelayedDeploys = new List<GridDeploy>();
    public GameObject staticSpaceHolder = null;
    public GameObject dynamicSpaceHolder = null;

    public DelayedLevelDeploy(GridSpace space, GameObject staticHolder, GameObject dynamicHolder)
    {
        this.space = space;
        this.staticSpaceHolder = staticHolder;
        this.dynamicSpaceHolder = dynamicHolder;
    }
}
