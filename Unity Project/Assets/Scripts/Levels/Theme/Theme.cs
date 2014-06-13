using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using LevelGen;

public abstract class Theme : ThemeOption, IInitializable
{
    #region Core Elements
    public ThemeElementBundle Wall;
    public ThemeElementBundle Door;
    public ThemeElementBundle Floor;
    public ThemeElementBundle Stair;
    public ThemeElementBundle Chest;
    #endregion
    [Copyable]
    private RoomModCollection roomMods;
    public RoomModCollection RoomMods { get { return roomMods; } }
    [Copyable]
    private SpawnModCollection spawnMods;
    public SpawnModCollection SpawnMods { get { return spawnMods; } }
    [Copyable]
    public SpawnKeywords[] Keywords;
    [Copyable]
    public GenericFlags<SpawnKeywords> KeywordFlags;
    [Copyable]
    public ProbabilityPool<ThemeMod> ThemeMods;
    public int MinThemeMods;
    public int MaxThemeMods;
    public int MinAreaMods = 1;
    public int MaxAreaMods = 1;
    public double AverageRoomRadius;
    public override double AvgRoomRadius
    {
        get
        {
            if (AverageRoomRadius == 0)
            {
                throw new ArgumentException(this + ": Average room radius == 0.  Did you forget to set it?");
            }
            return AverageRoomRadius;
        }
    }

    public virtual void Init()
    {
        roomMods = new RoomModCollection();
        spawnMods = new SpawnModCollection();
        KeywordFlags = new GenericFlags<SpawnKeywords>(Keywords);
        ThemeMods = ProbabilityPool<ThemeMod>.Create();
    }

    public override Theme GetTheme(System.Random rand)
    {
        return this;
    }

    public void ChooseAllSmartObjects(System.Random rand)
    {
        foreach (ThemeElementBundle bundle in this.FindAllDerivedObjects<ThemeElementBundle>(false))
        {
            bundle.Select(rand);
        }
    }

    public abstract bool GenerateRoom(LevelGenerator gen, Area a, LayoutObject<GenSpace> room);

    protected LayoutObject<GenSpace> ModRoom(LevelGenerator gen, Area a, LayoutObject<GenSpace> room)
    {
        #region DEBUG
        double time = 0;
        if (BigBoss.Debug.logging(Logs.LevelGenMain))
        {
            BigBoss.Debug.w(Logs.LevelGenMain, "Mods for " + room);
        }
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Modding " + room);
            time = Time.realtimeSinceStartup;
        }
        #endregion
        this.ChooseAllSmartObjects(gen.Rand);
        RoomMods.Freshen();
        RoomSpec spec = new RoomSpec(room, gen.Depth, this, gen.Rand);
        // Base Mod
        if (!ApplyMod(spec, spec.RoomModifiers.BaseMods))
        {
            throw new ArgumentException("Could not apply base mod");
        }
        // Definining Mod
        if (spec.RoomModifiers.AllowDefiningMod)
        {
            ApplyMod(spec, spec.RoomModifiers.DefiningMods);
        }
        // Flex Mods
        int numFlex = gen.Rand.Next(spec.RoomModifiers.MinFlexMods, spec.RoomModifiers.MaxFlexMods);
        int numHeavy = (int)Math.Round((numFlex / 3d) + (numFlex / 3d * gen.Rand.NextDouble()));
        int numFill = numFlex - numHeavy;
        // Heavy Mods
        for (int i = 0; i < numHeavy; i++)
        {
            if (!ApplyMod(spec, spec.RoomModifiers.HeavyMods))
            {
                break;
            }
        }
        // Fill Mods
        for (int i = 0; i < numFill; i++)
        {
            if (!ApplyMod(spec, spec.RoomModifiers.FillMods))
            {
                break;
            }
        }
        // Final Mods
        ApplyMod(spec, spec.RoomModifiers.FinalMods);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            room.ToLog(Logs.LevelGen);
            BigBoss.Debug.w(Logs.LevelGen, "Modding " + room + " took " + (Time.realtimeSinceStartup - time) + " seconds.");
            BigBoss.Debug.printFooter(Logs.LevelGen, "Modding " + room);
        }
        #endregion
        if (!ValidateRoom(room))
        {
            throw new ArgumentException(room + " is not valid.");
        }
        return room;
    }

    protected bool ApplyMod<T>(RoomSpec spec, ProbabilityPool<T> mods)
        where T : RoomModifier
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            mods.ToLog(BigBoss.Debug.Get(Logs.LevelGen), "Mod choices");
        }
        #endregion
        using (new ProbabilityTakeReverter<T>(mods))
        {
            T mod;
            while (mods.Take(spec.Random, out mod))
            {
                #region DEBUG
                float stepTime = 0;
                if (BigBoss.Debug.logging(Logs.LevelGenMain))
                {
                    BigBoss.Debug.w(Logs.LevelGenMain, "   Applying: " + mod);
                }
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    stepTime = Time.realtimeSinceStartup;
                    BigBoss.Debug.w(Logs.LevelGen, "Applying: " + mod);
                }
                #endregion
                Container2D<GenSpace> backupGrid = new MultiMap<GenSpace>(spec.Grids);
                if (mod.Modify(spec))
                {
                    #region DEBUG
                    if (BigBoss.Debug.logging(Logs.LevelGen))
                    {
                        spec.Grids.ToLog(Logs.LevelGen, "Applying " + mod + " took " + (Time.realtimeSinceStartup - stepTime) + " seconds.");
                    }
                    #endregion
                    return true;
                }
                else
                {
                    spec.Grids.Clear();
                    spec.Grids.PutAll(backupGrid);
                    #region DEBUG
                    if (BigBoss.Debug.logging(Logs.LevelGen))
                    {
                        spec.Grids.ToLog(Logs.LevelGen, "Couldn't apply mod.  Processing " + mod + " took " + (Time.realtimeSinceStartup - stepTime) + " seconds.");
                    }
                    #endregion
                }
            }
        }
        return false;
    }

    protected bool ValidateRoom(LayoutObject<GenSpace> room)
    {
        return room.Bounding.IsValid();
    }

    public override string ToString()
    {
        return name;
    }
}
