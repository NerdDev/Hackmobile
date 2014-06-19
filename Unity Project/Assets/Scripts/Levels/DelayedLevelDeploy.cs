using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DelayedLevelDeploy
{
    public int Counter;
    public List<GridDeploy> DelayedDeploys = new List<GridDeploy>();
    public GameObject staticSpaceHolder = null;
    public GameObject dynamicSpaceHolder = null;

    public DelayedLevelDeploy(GameObject staticHolder, GameObject dynamicHolder)
    {
        this.staticSpaceHolder = staticHolder;
        this.dynamicSpaceHolder = dynamicHolder;
    }
}
