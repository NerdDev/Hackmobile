using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AICore : IXmlParsable, ICopyable
{
    // Defining features
    public NPC Self { get; protected set; }
    public System.Random Random { get; protected set; }

    // Cores
    AIDecisionCore[] cores = new AIDecisionCore[EnumExt.Length<AIState>()];
    AIDecisionCore movementCore = new AIDecisionCore();

    // State variables
    public AIState CurrentState = AIState.Passive;
    public AIDecision LastDecision;
    public AIDecision CurrentDecision;
    private NPC _target;
    public NPC Target
    {
        get { return _target; }
        set
        {
            _target = value;
            if (_target != null)
            {
                TargetSpace = _target.GridSpace;
            }
        }
    }
    public Level Level
    {
        get { return Self.GridSpace.Level; }
    }
    public GridSpace TargetSpace;

    private bool processedNearbyNPCs = false;
    private List<NPC> friendlyNPCs = new List<NPC>();
    public IEnumerable<NPC> FriendlyNPCs
    {
        get
        {
            if (!processedNearbyNPCs)
            {
                ProcessNPCs();
            }
            return friendlyNPCs;
        }
    }
    private List<NPC> enemyNPCs = new List<NPC>();
    public IEnumerable<NPC> EnemyNPCs
    {
        get
        {
            if (!processedNearbyNPCs)
            {
                ProcessNPCs();
            }
            return enemyNPCs;
        }
    }
    private List<NPC> friendlyNPCsInLOS = new List<NPC>();
    public IEnumerable<NPC> FriendlyNPCsInLOS
    {
        get
        {
            if (!processedNearbyNPCs)
            {
                ProcessNPCs();
            }
            return friendlyNPCsInLOS;
        }
    }
    private List<NPC> enemyNPCsInLOS = new List<NPC>();
    public IEnumerable<NPC> EnemyNPCsInLOS
    {
        get
        {
            if (!processedNearbyNPCs)
            {
                ProcessNPCs();
            }
            return enemyNPCsInLOS;
        }
    }


    public AICore(NPC n)
    {
        this.Self = n;
        for (int i = 0; i < cores.Length; i++)
        {
            cores[i] = new AIDecisionCore();
        }
    }

    public void PostCopy()
    {
        Random = new System.Random(Probability.Rand.Next());
    }

    public bool Continuing(AIDecision decision)
    {
        return Object.ReferenceEquals(decision, LastDecision);
    }

    public void DecideWhatToDo()
    {
        ProbabilityPool<AIDecision> pool = ProbabilityPool<AIDecision>.Create();
        AIDecision decision;
        if (!cores[(int)CurrentState].FillPool(pool, this, out decision))
        {
            decision = pool.Get(Random);
        }
        #region Debug
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            Log log = BigBoss.Debug.CreateNewLog("AI/NPC " + Self.ID + "/Log.txt");
            log.printHeader("Deciding");
            pool.ToLog(log, "Decision options");
            log.w("Decided on " + decision.GetType().ToString());
            log.close();
        }
        #endregion
        CurrentDecision = decision;
        decision.Action(this);
        LastDecision = decision;
        #region Debug
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            Log log = BigBoss.Debug.CreateNewLog("AI/NPC " + Self.ID + "/Log.txt");
            log.printFooter("Deciding");
            log.close();
        }
        #endregion
    }

    protected void ProcessNPCs()
    {

    }

    public void MoveTo(int x, int y)
    {
        MoveTo(BigBoss.Levels.Level[x, y]);
    }

    public void MoveTo(GridSpace space)
    {
        ProbabilityPool<AIDecision> movementPool = ProbabilityPool<AIDecision>.Create();
        AIDecision movement;
        if (!movementCore.FillPool(movementPool, this, out movement))
        {
            movement = movementPool.Get(Random);
        }
        this.TargetSpace = space;
        movement.Action(this);
    }

    public void MoveTo(WorldObject wo)
    {
        MoveTo(wo.GridSpace);
    }

    #region XML
    public void ParseXML(XMLNode x)
    {
        foreach (var stateNode in x.SelectList("AIState"))
        {
            AIState state;
            if (!stateNode.SelectEnum<AIState>("State", out state)) continue;
            AIDecisionCore core = cores[(int)state];
            foreach (var packageNode in stateNode.SelectList("AIDecision"))
            {
                string name;
                if (!packageNode.SelectString("name", out name)) continue;
                AIDecision decision;
                if (!BigBoss.Types.TryInstantiate<AIDecision>(name, out decision)) continue;
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
            var core = cores[(int)AIState.Passive];
            core.AddDecision(new AIWait());
            // Combat
            core = cores[(int)AIState.Combat];
            core.AddDecision(new AIUseAbility());
        }
    }
    #endregion
}
