using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RoomModCollection
{
    private bool _allowDefiningMod = true;
    public bool AllowDefiningMod
    {
        get
        {
            return _allowDefiningMod && DefiningMods.Count > 0;
        }
        set { _allowDefiningMod = value; }
    }
    private bool _allowFinalMod = true;
    public bool AllowFinalMod
    {
        get
        {
            return _allowFinalMod && FinalMods.Count > 0;
        }
        set { _allowFinalMod = value; }
    }
    public int MinFlexMods = 3;
    public int MaxFlexMods = 6;
    public ProbabilityPool<BaseRoomMod> BaseMods = ProbabilityPool<BaseRoomMod>.Create();
    public ProbabilityPool<DefiningRoomMod> DefiningMods = ProbabilityPool<DefiningRoomMod>.Create();
    public ProbabilityPool<HeavyRoomMod> HeavyMods = ProbabilityPool<HeavyRoomMod>.Create();
    public ProbabilityPool<FillRoomMod> FillMods = ProbabilityPool<FillRoomMod>.Create();
    public ProbabilityPool<FinalRoomMod> FinalMods = ProbabilityPool<FinalRoomMod>.Create();

    public RoomModCollection()
    {
    }

    public RoomModCollection(RoomModCollection rhs)
    {
        BaseMods.AddAll(rhs.BaseMods);
        DefiningMods.AddAll(rhs.DefiningMods);
        HeavyMods.AddAll(rhs.HeavyMods);
        FillMods.AddAll(rhs.FillMods);
        FinalMods.AddAll(rhs.FinalMods);
    }

    public void AddMod(RoomModifier mod, double multiplier, bool unique = false)
    {
        if (mod is BaseRoomMod)
        {
            BaseMods.Add((BaseRoomMod)mod, multiplier, unique || mod.Unique);
        }
        else if (mod is DefiningRoomMod)
        {
            DefiningMods.Add((DefiningRoomMod)mod, multiplier, unique || mod.Unique);
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

    public void RemoveMod(RoomModifier mod, bool all = true)
    {
        if (mod is BaseRoomMod)
        {
            BaseMods.Remove((BaseRoomMod)mod, all);
        }
        else if (mod is DefiningRoomMod)
        {
            DefiningMods.Remove((DefiningRoomMod)mod, all);
        }
        else if (mod is HeavyRoomMod)
        {
            HeavyMods.Remove((HeavyRoomMod)mod, all);
        }
        else if (mod is FillRoomMod)
        {
            FillMods.Remove((FillRoomMod)mod, all);
        }
        else if (mod is FinalRoomMod)
        {
            FinalMods.Remove((FinalRoomMod)mod, all);
        }
        else
        {
            throw new ArgumentException("Cannot inherit directly from RoomModifier");
        }
    }
}

