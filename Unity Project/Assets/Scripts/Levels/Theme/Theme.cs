using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class Theme : MonoBehaviour
{
    public bool Scatterable;
    public bool Chainable;
    public ThemeElement[] WallElement;
    public GameObject[] Wall;
    public ThemeElement[] DoorElement;
    public GameObject[] Door;
    public ThemeElement[] FloorElement;
    public GameObject[] Floor;
    public ThemeElement[] StairUpElement;
    public GameObject[] StairUp;
    public ThemeElement[] StairDownElement;
    public GameObject[] StairDown;
    public ThemeElement[] PillarElement;
    public GameObject[] Pillar;
    public ThemeElement[] ChestElement;
    public GameObject[] Chest;
    public ProbabilityPool<BaseRoomMod> BaseMods = ProbabilityPool<BaseRoomMod>.Create();
    public ProbabilityPool<HeavyRoomMod> HeavyMods = ProbabilityPool<HeavyRoomMod>.Create();
    public ProbabilityPool<FillRoomMod> FillMods = ProbabilityPool<FillRoomMod>.Create();
    public ProbabilityPool<FinalRoomMod> FinalMods = ProbabilityPool<FinalRoomMod>.Create();
    public Keywords[] Keywords;
    public ESFlags<Keywords> KeywordFlags;

    public virtual void Init()
    {
        WallElement = Generate(Wall);
        DoorElement = Generate(Door);
        FloorElement = Generate(Floor);
        StairUpElement = Generate(StairUp);
        StairDownElement = Generate(StairDown);
        PillarElement = Generate(Pillar);
        ChestElement = Generate(Chest);
        KeywordFlags = (ESFlags<Keywords>)Keywords;
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

    protected ThemeElement[] Generate(GameObject[] objs)
    {
        ThemeElement[] ret = new ThemeElement[objs.Length];
        for (int i = 0; i < objs.Length; i++)
        {
            DontDestroyOnLoad(objs[i]);
            ret[i] = new ThemeElement(objs[i]);
        }
        return ret;
    }

    public ThemeElement Get(GridType t, System.Random rand)
    {
        switch (t)
        {
            case GridType.Door:
                return DoorElement.Random(rand);
            case GridType.Wall:
                return WallElement.Random(rand);
            case GridType.StairUp:
                return StairUpElement.Random(rand);
            case GridType.StairDown:
                return StairDownElement.Random(rand);
            case GridType.Chest:
                return ChestElement.Random(rand);
            case GridType.NULL:
                return null;
            default:
                return FloorElement.Random(rand);
        }
    }

}
