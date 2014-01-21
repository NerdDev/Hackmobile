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
        BigBoss.Levels.SetCurLevel(1);
        BigBoss.DungeonMaster.PopulateLevel(BigBoss.Levels.Level);

        // Temp (will move eventually)
        BigBoss.Debug.w(Logs.Main, "Placing player in initial position.");
        Level level = BigBoss.Levels.Level;
        Point stair = level.DownStartPoint;
        Value2D<GridSpace> start;
        level.Array.GetPointAround(stair.x, stair.y, false, Draw.IsType(GridType.StairPlace), out start);
        BigBoss.PlayerInfo.transform.position = new Vector3(start.x, -.5f, start.y);
        BigBoss.Debug.w(Logs.Main, "Placed player on " + start);
    }
}
