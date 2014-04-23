using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AICore : IXmlParsable, ICopyable
{
    // Defining features
    public NPC NPC;
    public double[] RoleWeights = new double[EnumExt.Length<AIRole>()];
    private AIDecisionArgs decisionArgs;
    private AIActionArgs actionArgs;
    public System.Random Random;
    public Func<AIDecision, double> WeightingCurve;

    // Cores
    AIRoleCore[] roleCores = new AIRoleCore[EnumExt.Length<AIState>()];
    AIDecisionCore movementCore = new AIDecisionCore();

    // State variables
    public AIState CurrentState = AIState.Passive;
    public AIDecision LastDecision;

    public AICore(NPC n)
    {
        this.NPC = n;
        for (int i = 0; i < roleCores.Length; i++)
        {
            roleCores[i] = new AIRoleCore();
        }
        decisionArgs = new AIDecisionArgs(this);
        actionArgs = new AIActionArgs(this);
        Random = new System.Random(Probability.Rand.Next());
    }

    public void PostCopy()
    {
        WeightingCurve = (decision) =>
        {
            double weight = decision.CalcWeighting(decisionArgs);
            if (Object.ReferenceEquals(decision, LastDecision))
            {
                weight += decision.StickyShift;
            }
            return weight;
        };
    }

    public void DecideWhatToDo()
    {
        decisionArgs.Target = BigBoss.Player; // Temp
        actionArgs.Target = BigBoss.Player; // Temp
        ProbabilityPool<AIDecision> pool = ProbabilityPool<AIDecision>.Create();
        roleCores[(int)CurrentState].FillPool(this, pool, decisionArgs);
        AIDecision decision = pool.Get(Random);
        #region Debug
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            Log log = BigBoss.Debug.CreateNewLog("AI/NPC " + decisionArgs.Self.ID + "/Log.txt");
            log.printHeader("Deciding");
            pool.ToLog(log, "Decision options");
            log.w("Decided on " + decision.GetType().ToString());
            log.close();
        }
        #endregion
        decision.Action(actionArgs);
        LastDecision = decision;
        #region Debug
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            Log log = BigBoss.Debug.CreateNewLog("AI/NPC " + decisionArgs.Self.ID + "/Log.txt");
            log.printFooter("Deciding");
            log.close();
        }
        #endregion
    }

    public void Move(int x, int y)
    {
        Move(BigBoss.Levels.Level[x, y]);
    }

    public void Move(GridSpace space)
    {
        ProbabilityPool<AIDecision> movementPool = ProbabilityPool<AIDecision>.Create();
        movementCore.FillPool(movementPool, decisionArgs);
        AIDecision movement = movementPool.Get(Random);
        movement.Action(new AIActionArgs(this) { TargetSpace = space });
    }

    #region XML
    public void ParseXML(XMLNode x)
    {
        foreach (var stateNode in x.SelectList("AIState"))
        {
            AIState state;
            if (!stateNode.SelectEnum<AIState>("State", out state)) continue;
            AIRoleCore core = roleCores[(int)state];
            foreach (var packageNode in stateNode.SelectList("AIDecision"))
            {
                string name;
                if (!x.SelectString("name", out name)) continue;
                AIRoleDecision decision;
                if (!BigBoss.Types.TryInstantiate<AIRoleDecision>(name, out decision)) continue;
                core.AddDecision(decision);
            }

        }

        foreach (var movementNode in x.SelectList("AIMovements"))
        {
            foreach (var packageNode in movementNode.SelectList("AIMovement"))
            {
                string name;
                if (!x.SelectString("name", out name)) continue;
                AIDecision movement;
                if (!BigBoss.Types.TryInstantiate<AIDecision>(name, out movement)) continue;
                movementCore.AddDecision(movement);
            }
        }

        // Package Defaults
        if (x.SelectBool("UseDefaults", true))
        {
            // Movement
            movementCore.AddDecision(new AIMove());
            // Passive
            var core = roleCores[(int)AIState.Passive];
            core.AddDecision(new AIAggro());
            core.AddDecision(new AIWait());
            // Combat
            core = roleCores[(int)AIState.Combat];
            core.AddDecision(new AIUseAbility());
        }
    }
    #endregion
}
