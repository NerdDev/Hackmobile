using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class Theme : MonoBehaviour
{
    public bool Scatterable;
    public bool Chainable;
    public ThemeElement[] Wall;
    public ThemeElement[] Door;
    public ThemeElement[] Floor;
    public ThemeElement[] StairUp;
    public ThemeElement[] StairDown;
    public ThemeElement[] Pillar;
    public ThemeElement[] Chest;
    public ProbabilityPool<BaseRoomMod> BaseMods;
    public ProbabilityPool<HeavyRoomMod> HeavyMods;
    public ProbabilityPool<FillRoomMod> FillMods;
    public ProbabilityPool<FinalRoomMod> FinalMods;
    public SpawnKeywords[] Keywords;
    public GenericFlags<SpawnKeywords> KeywordFlags;

    public virtual void Init()
    {
        BaseMods = ProbabilityPool<BaseRoomMod>.Create();
        HeavyMods = ProbabilityPool<HeavyRoomMod>.Create();
        FillMods = ProbabilityPool<FillRoomMod>.Create();
        FinalMods = ProbabilityPool<FinalRoomMod>.Create();
        KeywordFlags = new GenericFlags<SpawnKeywords>(Keywords);
    }

    protected void AddMod(RoomModifier mod, double multiplier, bool unique = false)
    {
        if (mod is BaseRoomMod)
        {
            BaseMods.Add((BaseRoomMod)mod, multiplier, unique || mod.Unique);
        }
        else if (mod is HeavyRoomMod)
        {
            HeavyMods.Add((HeavyRoomMod)mod, multiplier, unique || mod.Unique);
        }
        else if (mod is FillRoomMod)
        {
            FillMods.Add((FillRoomMod)mod, multiplier, unique || mod.Unique);
        }
        else if (mod is FinalRoomMod)
        {
            FinalMods.Add((FinalRoomMod)mod, multiplier, unique || mod.Unique);
        }
        else
        {
            throw new ArgumentException("Cannot inherit directly from RoomModifier");
        }
    }

    public ThemeElement Get(GridType t, System.Random rand)
    {
        switch (t)
        {
            case GridType.Door:
                return Door.Random(rand);
            case GridType.Wall:
                return Wall.Random(rand);
            case GridType.StairUp:
                return StairUp.Random(rand);
            case GridType.StairDown:
                return StairDown.Random(rand);
            case GridType.Chest:
                return Chest.Random(rand);
            case GridType.NULL:
                return null;
            default:
                return Floor.Random(rand);
        }
    }

}
