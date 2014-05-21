using LevelGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UndeadTombTheme : Theme, IPillarTheme, IClusteringTheme
{
    public ThemeElementBundle Pillars;
    public ThemeElementBundle Tombs;
    public double ClusterSplitPercent = .25d;
    public double ClusterSplitPercentProperty { get { return ClusterSplitPercent; } }

    public override void Init()
    {
        base.Init();
        // Base
        RoomMods.AddMod(new CircleRoom(), .5d);
        RoomMods.AddMod(new RectangularRoom(), 2d);
        RoomMods.AddMod(new SquareRoom(), 1d);
        RoomMods.AddMod(new TRoomMod(), 1d);
        //RoomMods.AddMod(new GrandTombRoom(), 0.5d, true);
        // Defining
        RoomMods.AddMod(new BlankDefiningRoomMod(), 1);
        // Flex
        RoomMods.AddMod(new MassTombRoom(), 1, true);
        RoomMods.AddMod(new GiantPillarMod(), 1);
        RoomMods.AddMod(new HiddenRoomMod(), .5);
        RoomMods.AddMod(new PillarMod(), 1);
        RoomMods.AddMod(new SplitterMod(), 1);
        RoomMods.AddMod(new TrapRoomMod(), .2d);
        RoomMods.AddMod(new TreasureRoomMod(), .3d);
    }

    public ThemeElementBundle GetPillars()
    {
        return Pillars;
    }

    public override bool GenerateRoom(LevelGenerator gen, Area a, out LayoutObject<GenSpace> room)
    {
        room = CreateRoom(gen, a);
        if (!this.PlaceOrClusterAround(gen, a, room))
        {
            return false;
        }
        return true;
    }
}