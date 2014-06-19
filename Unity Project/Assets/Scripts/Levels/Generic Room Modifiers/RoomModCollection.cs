using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RoomModCollection
{
    public int MinFlexMods = 3;
    public int MaxFlexMods = 6;
    [Copyable]
    public ProbabilityPool<BaseRoomMod> BaseMods = ProbabilityPool<BaseRoomMod>.Create();
    [Copyable]
    public ProbabilityPool<RoomModifier> HeavyMods = ProbabilityPool<RoomModifier>.Create();
    [Copyable]
    public ProbabilityPool<RoomModifier> FillMods = ProbabilityPool<RoomModifier>.Create();
    [Copyable]
    public List<RoomModifier> DecorationMods = new List<RoomModifier>();

    public RoomModCollection()
    {
    }

    public RoomModCollection(RoomModCollection rhs)
    {
        BaseMods.AddAll(rhs.BaseMods);
        HeavyMods.AddAll(rhs.HeavyMods);
        FillMods.AddAll(rhs.FillMods);
        DecorationMods.AddRange(rhs.DecorationMods);
    }

    public void AddMod(RoomModifier mod, double multiplier, bool unique = false)
    {
        switch (mod.Type)
        {
            case RoomModifierType.Base:
                BaseMods.Add((BaseRoomMod)mod, multiplier, unique || mod.Unique);
                break;
            case RoomModifierType.Heavy:
                HeavyMods.Add(mod, multiplier, unique || mod.Unique);
                break;
            case RoomModifierType.Fill:
                FillMods.Add(mod, multiplier, unique || mod.Unique);
                break;
            case RoomModifierType.Decoration:
            default:
                DecorationMods.Add(mod);
                break;
        }
    }

    public void RemoveMod(RoomModifier mod, bool all = true)
    {
        switch (mod.Type)
        {
            case RoomModifierType.Base:
                BaseMods.Remove((BaseRoomMod)mod, all);
                break;
            case RoomModifierType.Heavy:
                HeavyMods.Remove(mod, all);
                break;
            case RoomModifierType.Fill:
                FillMods.Remove(mod, all);
                break;
            case RoomModifierType.Decoration:
            default:
                DecorationMods.Remove(mod);
                break;
        }
    }

    public void Freshen()
    {
        BaseMods.Freshen();
        HeavyMods.Freshen();
        FillMods.Freshen();
    }
}

