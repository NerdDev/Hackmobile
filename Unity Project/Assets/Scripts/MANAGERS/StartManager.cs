using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class StartManager : MonoBehaviour, IManager
{

    public bool Initialized { get; set; }
    public void Initialize()
    {

    }

    public void Start()
    {
        BigBoss.Levels.SetCurLevel(0);
        BigBoss.DungeonMaster.PopulateLevel(BigBoss.Levels.Level, false);
    }
}
