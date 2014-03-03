using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RoomModCollection
{
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

    protected List<R> PickAndIntegrate<R>(ProbabilityPool<R> pool, int number, System.Random Rand)
        where R : RoomModifier
    {
        List<R> ret = new List<R>(number);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            pool.ToLog(Logs.LevelGen, typeof(R).ToString() + " Probability");
        }
        #endregion
        ret.AddRange(pool.Get(Rand, number));
        foreach (R r in ret)
        {
            foreach (ProbabilityItem<RoomModifier> chained in r.GetChainedModifiers())
            {
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    BigBoss.Debug.w(Logs.LevelGen, "Chained: " + chained);
                }
                #endregion
                AddMod(chained.Item, chained.Multiplier, chained.Unique);
            }
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Picked: ");
            foreach (R r in ret)
            {
                BigBoss.Debug.w(Logs.LevelGen, "  " + r.ToString());
            }
        }
        #endregion
        return ret;
    }

    protected bool PickAndIntegrate<R>(ProbabilityPool<R> pool, System.Random Rand, out R picked)
        where R : RoomModifier
    {
        List<R> list = PickAndIntegrate(pool, 1, Rand);
        if (list.Count > 0)
        {
            picked = list[0];
            return true;
        }
        picked = null;
        return false;
    }

    public List<RoomModifier> PickMods(System.Random Rand)
    {
        RoomModCollection tmp = new RoomModCollection(this);
        List<RoomModifier> mods = new List<RoomModifier>();
        // Base Mod
        BaseRoomMod baseMod;
        PickAndIntegrate(tmp.BaseMods, Rand, out baseMod);
        mods.Add(baseMod);
        // Definining Mod
        if (baseMod.AllowDefiningMod)
        {
            mods.AddRange(PickAndIntegrate(tmp.DefiningMods, 1, Rand).Cast<RoomModifier>());
        }
        // Flex Mods
        int numFlex = Rand.Next(baseMod.MaxFlexMods, baseMod.MaxFlexMods);
        int numHeavy = (int)Math.Round((numFlex / 3d) + (numFlex / 3d * Rand.NextDouble()));
        int numFill = numFlex - numHeavy;
        // Heavy Mods
        mods.AddRange(PickAndIntegrate(tmp.HeavyMods, numHeavy, Rand).Cast<RoomModifier>());
        // Fill Mods
        mods.AddRange(PickAndIntegrate(tmp.FillMods, numFill, Rand).Cast<RoomModifier>());
        // Final Mods
        mods.AddRange(PickAndIntegrate(tmp.FinalMods, 1, Rand).Cast<RoomModifier>());
        return mods;
    }
}

