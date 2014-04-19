using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AICore : IXmlParsable
{
    // Defining features
    public NPC NPC;
    public double[] RoleWeights = new double[EnumExt.Length<AIRole>()];
    public Func<double, double> WeightingCurve;
    private AIDecisionArgs decisionArgs;
    private AIActionArgs actionArgs;
    private System.Random rand;

    // State variables
    AIRoleCore[] roleCores = new AIRoleCore[EnumExt.Length<AIState>()];
    AIMovementCore movementCore = new AIMovementCore();
    public AIState CurrentState = AIState.Passive;

    public AICore(NPC n)
    {
        this.NPC = n;
        for (int i = 0; i < roleCores.Length; i++)
        {
            roleCores[i] = new AIRoleCore();
        }
        decisionArgs = new AIDecisionArgs(this);
        actionArgs = new AIActionArgs(this);
        rand = new System.Random(Probability.Rand.Next());
        WeightingCurve = (weight) =>
        {
            return weight + 1;
        };
    }

    public void DecideWhatToDo()
    {
        ProbabilityPool<AIRoleDecision> pool = ProbabilityPool<AIRoleDecision>.Create();
        roleCores[(int)CurrentState].FillPool(this, pool, decisionArgs);
        AIRoleDecision decision = pool.Get(rand);
        decision.Action(actionArgs);
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
                AIMovement movement;
                if (!BigBoss.Types.TryInstantiate<AIMovement>(name, out movement)) continue;
                movementCore.AddMovement(movement);
            }
        }

        // Package Defaults
        if (x.SelectBool("UseDefaults", true))
        {
            // Movement
            movementCore.AddMovement(new AIMove());
            // Passive
            var core = roleCores[(int)AIState.Passive];
            core.AddDecision(new AIAggro());
            core.AddDecision(new AIWait());
            // Combat
            core = roleCores[(int)AIState.Combat];
            core.AddDecision(new AIAttack());
        }
    }
    #endregion
}
