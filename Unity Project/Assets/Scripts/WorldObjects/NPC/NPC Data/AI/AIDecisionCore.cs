using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIDecisionCore
{
    public AIDecision LastDecision { get; protected set; }
    public AIDecision CurrentDecision { get; protected set; }
    public HashSet<AIDecision> Decisions = new HashSet<AIDecision>();
    AICore core;

    public AIDecisionCore(AICore core)
    {
        this.core = core;
    }

    public void ExecuteDecision()
    {
        AIDecision decision = FillPool();
        CurrentDecision = decision;
        if (decision.Args.Actions != null)
        {
            decision.Args.Actions(core);
        }
        LastDecision = decision;
    }

    public bool Continuing(AIDecision decision)
    {
        if (decision.Args.Ending) return false;
        var last = LastDecision;
        while (last != null)
        {
            if (System.Object.ReferenceEquals(decision, last)) return true;
            last = last.Args.LastPassedDecision;
        }
        return false;
    }

    protected AIDecision FillPool()
    {
        double reducLevel = 0, reducAmount = 0;
        if (LastDecision != null)
        {
            reducAmount = LastDecision.Args.StickyReduc;
            reducLevel = LastDecision.Args.Weight;
        }
        bool reducing = reducAmount > 0d;
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI) && reducing)
        {
            core.Log.w("Reducing " + reducAmount + " below weight " + reducLevel);
        }
        #endregion
        ProbabilityPool<AIDecision> pool = ProbabilityPool<AIDecision>.Create();
        foreach (AIDecision decision in Decisions)
        {
            decision.Args.Reset();
            #region DEBUG
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
            {
                core.Log.printHeader(decision.GetType().Name);
            }
            #endregion
            if (!decision.Decide(core, this))
            {
                #region DEBUG
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
                {
                    core.Log.printFooter(decision.GetType().Name);
                }
                #endregion
                continue;
            }
            double weight = decision.Args.Weight;
            if (double.IsInfinity(weight))
            { // Instant picking
                #region DEBUG
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
                {
                    core.Log.w("Instant Picking: " + decision);
                    core.Log.printFooter(decision.GetType().Name);
                }
                #endregion
                return decision;
            }
            if (Continuing(decision))
            {
                weight += decision.Args.StickyShift;
            }
            if (reducing && weight < reducLevel)
            {
                weight /= reducAmount * (1 - (weight / reducLevel));
            }
            #region DEBUG
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
            {
                decision.Args.ToLog(core.Log);
                core.Log.w("Final weight: " + weight);
                core.Log.printFooter(decision.GetType().Name);
            }
            #endregion
            pool.Add(decision, weight);
        }
        var chosen = pool.Get(core.Random);
        #region Debug
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            pool.ToLog(core.Log, "Decision options");
            core.Log.w("Decided on " + chosen.GetType().ToString());
        }
        #endregion
        return chosen;
    }

    public void AddDecision(AIDecision decision)
    {
        Decisions.Add(decision);
    }
}