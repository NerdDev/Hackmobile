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
        //Level level;
        //BigBoss.Levels.GetLevel(0, out level);
        BigBoss.Levels.SetCurLevel(0);
        BigBoss.DungeonMaster.PopulateLevel(BigBoss.Levels.Level);
        //BigBoss.Levels.GetLevel(1, out level);


        // Temp (will move eventually)
        RandomPicker<GridSpace> picker;
        Level level = BigBoss.Levels.Level;
        level.DownStairs.Select(true);
        if (level.UpStairs != null)
            level.UpStairs.Select(false);
        Point stair = level.DownStairs.SelectedLink;
        level.Array.DrawAround(stair.x, stair.y, false,
            Draw.IfThen<GridSpace>((arr, x, y) =>
                {
                    return arr[x, y].Type == GridType.Floor;
                },
            Draw.PickRandom(out picker)));
        Value2D<GridSpace> pt = picker.Pick(new System.Random());
        BigBoss.PlayerInfo.transform.position = new Vector3(pt.x, -.5f, pt.y);
    }
}
