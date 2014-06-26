using LevelGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UndeadTombTheme : Theme, IPillarTheme, IClusteringTheme, ISpikeTrapTheme
{
    public ThemeElementBundle Pillars;
    public ThemeElementBundle SpikeTraps;
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
        // Flex
        RoomMods.AddMod(new MassTombRoom(), 0.35d, true);
        RoomMods.AddMod(new GiantPillarMod(), 1);
        //RoomMods.AddMod(new HiddenRoomMod(), .5);
        RoomMods.AddMod(new PillarMod(), 1);
        RoomMods.AddMod(new SplitterMod(), 1);
        //RoomMods.AddMod(new SpikeTrapRoomMod(), .1d);
        //RoomMods.AddMod(new TreasureRoomMod(), .15d);

        /*
         * Spawn Mods
         */
        SpawnMods.RoomMods.Add(new SpawnNPCs());

        /*
         * Theme Mods
         */
        MinThemeMods = 1;
        MaxThemeMods = 1;
        ThemeMods.Add(new WispSpiritThemeMod(), .4);
    }

    public ThemeElementBundle GetPillars()
    {
        return Pillars;
    }

    public ThemeElementBundle GetSpikeTraps()
    {
        return SpikeTraps;
    }

    public override bool GenerateRoom(LevelGenerator gen, Area a, LayoutObject<GenSpace> room)
    {
        ModRoom(gen, a, room);
        if (!this.PlaceOrClusterAround(gen, a, room))
        {
            return false;
        }
        return true;
    }
}