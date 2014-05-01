using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AICore : IXmlParsable, ICopyable
{
    // Defining features
    public NPC Self { get; protected set; }
    public System.Random Random { get; protected set; }

    // Cores
    AIDecisionCore[] cores = new AIDecisionCore[EnumExt.Length<AIState>()];

    // State variables
    public AIState CurrentState = AIState.Passive;
    public AIDecision LastDecision;
    public AIDecision CurrentDecision;
    private WorldObject _target;
    public WorldObject Target
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

    #region NPC Presence Memory
    public int NumFriendlies;
    public int NumEnemies;
    public NPC ClosestEnemy;
    public double ClosestEnemyDist;
    public Dictionary<NPC, NPCMemoryItem> NPCMemory = new Dictionary<NPC, NPCMemoryItem>();
    #endregion

    #region Movement Memory
    public Dictionary<GridSpace, GridSpace> MovementSubstitutions = new Dictionary<GridSpace, GridSpace>();
    #endregion

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
        return System.Object.ReferenceEquals(decision, LastDecision);
    }

    public void DecideWhatToDo()
    {
        Reset();
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

    protected void Reset()
    {
        UpdateNPCMemory();
        MovementSubstitutions.Clear();
    }

    protected void UpdateNPCMemory()
    {
        NumFriendlies = 0;
        NumEnemies = 0;

        // Update NPCs already aware of
        foreach (var item in NPCMemory.Values.ToList())
        {
            // Update can see
            item.CanSee = Self.CanSee(item.NPC);
            // Update NPC memory item
            UpdateNPCMemory(item);
            // Remove if should forget
            if (!item.CanSee && ShouldForget(item))
            {
                NPCMemory.Remove(item.NPC);
                break;
            }
            // Update faction info
            if (item.AwareOf)
            {
                if (item.Friendly)
                {
                    NumFriendlies++;
                }
                else
                {
                    double dist = Vector3.Distance(Self.GO.transform.position, item.NPC.GO.transform.position);
                    if (dist > ClosestEnemyDist)
                    {
                        ClosestEnemy = item.NPC;
                        ClosestEnemyDist = dist;
                    }
                    NumEnemies++;
                }
            }
        }

        // Check for new NPCs to become aware of
        foreach (WorldObject wo in BigBoss.Levels.Level.WorldObjects)
        {
            NPC npc = wo as NPC;
            if (npc == null) continue;
            if (NPCMemory.ContainsKey(npc)) continue;
            if (!Self.CanSee(npc)) continue;
            NPCMemoryItem item = new NPCMemoryItem(npc);
            item.CanSee = true;
            NPCMemory[npc] = item;
            UpdateNPCMemory(item);
        }
    }

    protected void UpdateNPCMemory(NPCMemoryItem item)
    {
        if (item.CanSee)
        {
            if (!item.AwareOf && ShouldBecomeAware(item))
            {
                item.AwareOf = true;
            }
            if (item.AwareOf)
            {
                item.SpaceLastSeen = item.NPC.GridSpace;
                item.TurnLastSeen = BigBoss.Time.CurrentTurn;
                item.Friendly = IsFriendly(item);
            }
        }
    }

    protected bool ShouldBecomeAware(NPCMemoryItem item)
    {
        // Improve later
        return Random.Percent(.75d);
    }

    protected bool ShouldForget(NPCMemoryItem item)
    {
        // Improve later
        return BigBoss.Time.CurrentTurn - item.TurnLastSeen > 200;
    }

    protected bool IsFriendly(NPCMemoryItem item)
    {
        return !System.Object.ReferenceEquals(BigBoss.Player, item.NPC);
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
        this.Target = null;
        GridSpace sub;
        if (MovementSubstitutions.TryGetValue(space, out sub))
        {
            this.TargetSpace = sub;
        }
        else
        {
            this.TargetSpace = space;
        }
        Move();
    }

    public void MoveTo(WorldObject wo)
    {
        GridSpace sub;
        if (MovementSubstitutions.TryGetValue(wo.GridSpace, out sub))
        {
            this.Target = null;
            this.TargetSpace = sub;
            Move();
        }
        else
        {
            this.TargetSpace = null;
            this.Target = wo;
            Move();
        }
    }

    protected void Move()
    {
        ProbabilityPool<AIDecision> movementPool = ProbabilityPool<AIDecision>.Create();
        AIDecision movement;
        if (!cores[(int)AIState.Movement].FillPool(movementPool, this, out movement))
        {
            movement = movementPool.Get(Random);
        }
        movement.Action(this);
    }

    public void MoveAway(int x, int y, float range = 10)
    {
        MoveAway(BigBoss.Levels.Level[x, y], range);
    }

    public void MoveAway(GridSpace space, float range = 10)
    {
        GridSpace target = null;
        double distance = 0, cur = 0;
        Level.DrawBreadthFirstFill(space.X, space.Y, true,
            Draw.Walkable<GridSpace>().And((arr, x, y) =>
            {
                cur = Math.Sqrt(Math.Pow(x - space.X, 2) + Math.Pow(y - space.Y, 2));
                if (cur > distance)
                {
                    distance = cur;
                    target = arr[x, y];
                }
                return true;
            }).And(Draw.WithinTo<GridSpace>(range, space)));
        if (target != null)
        {
            MoveTo(target);
        }
    }

    public void MoveAway(WorldObject wo, float range = 10)
    {
        MoveAway(wo.GridSpace, range);
    }

    #region XML
    public void ParseXML(XMLNode x)
    {
        foreach (var packageNode in x.SelectList("AIDecision"))
        {
            AIDecision decision;
            if (ParseDecision(packageNode, out decision))
            {
                foreach (AIState state in decision.States)
                {
                    cores[(int)state].AddDecision(decision);
                }
            }
        }

        // Package Defaults
        if (x.SelectBool("UseDefaults", true))
        {
            AIDecisionCore core;
            // Movement
            core = cores[(int)AIState.Movement];
            core.AddDecision(new AIMove());
            // Passive
            core = cores[(int)AIState.Passive];
            core.AddDecision(new AIWait());
            // Combat
            core = cores[(int)AIState.Combat];
            core.AddDecision(new AIWait());
            core.AddDecision(new AIUseAbility());
        }
    }

    protected bool ParseDecision(XMLNode node, out AIDecision decision)
    {
        string name;
        if (!node.SelectString("name", out name))
        {
            decision = null;
            return false;
        }
        if (BigBoss.Types.TryInstantiate<AIDecision>(name, out decision))
        {
            decision.ParseXML(node);
            return true;
        }
        return false;
    }
    #endregion
}
