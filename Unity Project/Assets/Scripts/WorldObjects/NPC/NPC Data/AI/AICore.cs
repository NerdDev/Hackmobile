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
    }

    public void DecideWhatToDo()
    {
        ProbabilityPool<AIDecision> pool = ProbabilityPool<AIDecision>.Create();
        roleCores[(int)CurrentState].FillPool(this, pool, decisionArgs);
        AIDecision decision = pool.Get(rand);
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
            foreach (var packageNode in stateNode.SelectList("AIPackage"))
            {
                string name;
                if (!x.SelectString("name", out name)) continue;
                AIDecision decision;
                if (!BigBoss.Types.TryInstantiate<AIDecision>(name, out decision)) continue;
                core.AddDecision(decision);
            }

        }

        // Package Defaults
        if (x.SelectBool("UseDefaults", true))
        {
            // Passive
            var core = roleCores[(int)AIState.Passive];
            core.AddDecision(new AIAggro());
            // Combat
            core = roleCores[(int)AIState.Combat];
            core.AddDecision(new AIAttack());
            core.AddDecision(new AIMove());
        }
    }
    #endregion
}
