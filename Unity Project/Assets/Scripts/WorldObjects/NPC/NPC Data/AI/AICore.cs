using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public delegate void DecisionActions(AICore core);

public class AICore : IXmlParsable, ICopyable
{
    // Defining features
    public NPC Self { get; protected set; }
    public System.Random Random { get; protected set; }

    // Cores
    public HashSet<AIDecision> DecisionSet = new HashSet<AIDecision>();
    AIDecisionCore[] cores = new AIDecisionCore[EnumExt.Length<AIState>()];

    // State variables
    public AIState CurrentState = AIState.Passive;
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
    public GridSpace LastMovementGoal;

    #region NPC Presence Memory
    public int NumFriendlies;
    public int NumEnemies;
    public NPC ClosestEnemy;
    public double ClosestEnemyDist;
    public Dictionary<NPC, NPCMemoryItem> NPCMemory = new Dictionary<NPC, NPCMemoryItem>();
    private Dictionary<AIDecision, List<NPCMemoryItem>> decisionMemoryCache = new Dictionary<AIDecision, List<NPCMemoryItem>>();
    #endregion

    #region Movement Memory
    public Dictionary<GridSpace, GridSpace> MovementSubstitutions = new Dictionary<GridSpace, GridSpace>();
    #endregion

    public Log Log;

    public AICore(NPC n)
    {
        this.Self = n;
        for (int i = 0; i < cores.Length; i++)
        {
            cores[i] = new AIDecisionCore(this);
        }
    }

    public void PostPrimitiveCopy()
    {
    }

    public void PostObjectCopy()
    {
        Random = new System.Random(Probability.Rand.Next());
        Log = BigBoss.Debug.CreateNewLog("AI/NPC " + Self.ID + "/Log.txt");
    }

    public void DecideWhatToDo()
    {
        #region Debug
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            Log.printHeader("Deciding - Turn " + BigBoss.Time.CurrentTurn + " - " + System.DateTime.Now + " - " + Self.GridSpace + " - State: " + CurrentState);
        }
        #endregion
        Reset();
        #region Debug
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            ToLog(Log);
        }
        #endregion
        cores[(int)CurrentState].ExecuteDecision();
        #region Debug
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            Log.printFooter("Deciding");
        }
        #endregion
    }

    protected void Reset()
    {
        NumFriendlies = 0;
        NumEnemies = 0;
        ClosestEnemyDist = 0;
        ClosestEnemy = null;
        UpdateNPCMemory();
        MovementSubstitutions.Clear();
        decisionMemoryCache.Clear();
    }

    public bool Contains(AIDecision decision)
    {
        return DecisionSet.Contains(decision);
    }

    #region NPC Memory
    protected void UpdateNPCMemory()
    {
        #region Debug
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            Log.printHeader("UpdateNPCMemory");
        }
        #endregion
        // Check for new NPCs to become aware of
        foreach (WorldObject wo in BigBoss.Levels.Level.WorldObjects)
        {
            NPC npc = wo as NPC;
            if (npc == null) continue;
            if (System.Object.ReferenceEquals(Self, npc)) continue;
            if (NPCMemory.ContainsKey(npc)) continue;
            if (!Self.CanSee(npc)) continue;
            NPCMemoryItem item = new NPCMemoryItem(npc);
            NPCMemory[npc] = item;
            #region Debug
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
            {
                Log.w("Adding to memory: " + npc);
            }
            #endregion
        }

        // Update NPCs already aware of
        foreach (var item in NPCMemory.Values.ToList())
        {
            // Update can see
            item.CanSee = Self.CanSee(item.NPC);
            #region Debug
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
            {
                Log.printHeader(item.NPC.ToString());
            }
            #endregion
            // Update NPC memory item
            UpdateNPCMemory(item);
            #region Debug
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
            {
                item.ToLog(Log);
            }
            #endregion
            // Remove if should forget
            if (!item.CanSee && ShouldForget(item))
            {
                #region Debug
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
                {
                    Log.printFooter("Forgetting: " + item.NPC);
                }
                #endregion
                NPCMemory.Remove(item.NPC);
                continue;
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
            #region Debug
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
            {
                Log.printFooter(item.NPC.ToString());
            }
            #endregion
        }
        #region Debug
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            Log.printFooter("Update NPC Memory");
        }
        #endregion
    }

    protected void UpdateNPCMemory(NPCMemoryItem item)
    {
        if (item.CanSee)
        {
            if (!item.AwareOf && ShouldBecomeAware(item))
            {
                item.AwareOf = true;
                #region Debug
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
                {
                    Log.w("Became Aware");
                }
                #endregion
            }
            if (item.AwareOf)
            {
                item.SpaceLastSeen = item.NPC.GridSpace;
                item.TurnLastSeen = BigBoss.Time.CurrentTurn;
            }
        }
        item.Friendly = IsFriendly(item);
    }

    protected bool ShouldBecomeAware(NPCMemoryItem item)
    {
        // Improve later
        return Random.Percent(.75d);
    }

    protected bool ShouldForget(NPCMemoryItem item)
    {
        // Improve later
        return item.TurnsSinceLastSeen > 200;
    }

    protected bool IsFriendly(NPCMemoryItem item)
    {
        return !System.Object.ReferenceEquals(BigBoss.Player, item.NPC);
    }

    public List<NPCMemoryItem> GetNPCsUsingDecision(AIDecision decision)
    {
        List<NPCMemoryItem> list;
        if (!decisionMemoryCache.TryGetValue(decision, out list))
        {
            list = new List<NPCMemoryItem>(NPCMemory.Values.Count);
            foreach (NPCMemoryItem mem in NPCMemory.Values)
            {
                if (mem.NPC.AI.Contains(decision))
                {
                    list.Add(mem);
                }
            }
        }
        return list;
    }

    public void LearnAbout(NPCMemoryItem item)
    {
        if (item.NPC.Equals(Self)) return;
        NPCMemoryItem cur;
        if (NPCMemory.TryGetValue(item.NPC, out cur))
        {
            cur.Absorb(item);
        }
        else
        {
            cur = new NPCMemoryItem(item);
            NPCMemory[item.NPC] = cur;
        }
    }
    #endregion

    #region Move
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
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            Log.w("Trying to move to " + space);
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
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
            {
                Log.w("Trying to move to " + sub);
            }
            Move();
        }
        else
        {
            this.TargetSpace = wo.GridSpace;
            this.Target = wo;
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
            {
                Log.w("Trying to move to " + wo + " at space " + wo.GridSpace);
            }
            Move();
        }
    }

    protected void Move()
    {
        cores[(int)AIState.Movement].ExecuteDecision();
        LastMovementGoal = TargetSpace;
    }

    public void MoveAway(int x, int y, float range = 4)
    {
        MoveAway(BigBoss.Levels.Level[x, y], range);
    }

    public void MoveAway(GridSpace space, float range = 4)
    {
        GridSpace target = null;
        double toFleeTargetDist = 0, toTarget = 0, potentialFleeTargetDist = 0, potentialToTarget = 0;
        var draw = Draw.Walkable<GridSpace>().And((arr, x, y) =>
            {
                potentialFleeTargetDist = Math.Sqrt(Math.Pow(x - space.X, 2) + Math.Pow(y - space.Y, 2));
                potentialToTarget = Math.Sqrt(Math.Pow(x - Self.GridSpace.X, 2) + Math.Pow(y - Self.GridSpace.Y, 2));
                if (potentialFleeTargetDist > toFleeTargetDist || (potentialFleeTargetDist == toFleeTargetDist && potentialToTarget < toFleeTargetDist))
                {
                    toFleeTargetDist = potentialFleeTargetDist;
                    target = arr[x, y];
                }
                return true;
            }).And(Draw.WithinTo<GridSpace>(range, Self.GridSpace));
        #region DEBUG
        MultiMap<GridSpace> tmp = null;
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            tmp = new MultiMap<GridSpace>();
            draw = draw.And(Draw.AddTo(tmp));
        }
        #endregion
        Level.DrawBreadthFirstFill(Self.GridSpace.X, Self.GridSpace.Y, true, draw);
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            MultiMap<GridType> tmp2 = new MultiMap<GridType>();
            tmp.DrawAll((arr, x, y) =>
            {
                arr.DrawRect(x, y, 5, Draw.AddGridTo<GridSpace, GridSpace>(Level, tmp2));
                return true;
            });
            tmp.DrawAll(Draw.SetTo<GridSpace>(tmp2, GridType.INTERNAL_RESERVED_BLOCKED));
            if (target != null)
            {
                tmp2[target] = GridType.INTERNAL_RESERVED_CUR;
            }
            tmp2[space] = GridType.INTERNAL_MARKER_1;
            tmp2[Self.GridSpace] = GridType.INTERNAL_MARKER_2;
            tmp2.ToLog(Log, "Move away area");
        }
        #endregion
        if (target != null)
        {
            MoveTo(target);
        }
    }

    public void MoveAway(WorldObject wo, float range = 4)
    {
        MoveAway(wo.GridSpace, range);
    }

    public void FleeNPCs(Func<NPCMemoryItem, bool> evalFunc, float fleeRadius, float lookRadius = 4, float scatterWeight = 0.15f, float travelDistanceWeight = 0.02f)
    {
        GridSpace target = null;
        double bestWeight = double.MinValue;
        float maxRadius = Math.Max(fleeRadius, lookRadius);

        #region NPC Filter
        // Filter out NPCs that won't have an effect
        List<NPCMemoryItem> filteredNPCMem = new List<NPCMemoryItem>();
        foreach (var npcMem in NPCMemory.Values)
        {
            if (!npcMem.CanSee) continue;
            double dist = npcMem.NPC.GridSpace.Distance(Self.GridSpace);
            if (dist <= maxRadius)
            { // If inside radius
                filteredNPCMem.Add(npcMem);
            }
        }
        #endregion

        #region Weight Func
        Func<Container2D<GridSpace>, int, int, double> weightFunc = (arr, x, y) =>
        {
            double weight = 0;
            // Favor closer options
            weight -= Math.Sqrt(Math.Pow(x - Self.GridSpace.X, 2) + Math.Pow(y - Self.GridSpace.Y, 2)) * travelDistanceWeight;
            foreach (var npcMem in filteredNPCMem)
            {
                if (npcMem.NPC.GridSpace.X == x && npcMem.NPC.GridSpace.Y == y) return double.MinValue;
                // Get NPC distance from target space
                double npcDist = maxRadius - npcMem.SpaceLastSeen.Distance(x, y);
                double npcWeight = npcDist / maxRadius;
                if (!evalFunc(npcMem))
                {
                    npcWeight *= scatterWeight;
                }
                // Add NPC distance to weighting
                weight -= npcWeight;
            }
            return weight;
        };
        #endregion

        var draw = Draw.Walkable<GridSpace>().And((arr, x, y) =>
        {
            double weight = weightFunc(arr, x, y);
            if (weight > bestWeight)
            {
                bestWeight = weight;
                target = arr[x, y];
            }
            return true;
        }).And(Draw.WithinTo<GridSpace>(lookRadius, Self.GridSpace));
        #region DEBUG
        MultiMap<double> weightMap = null;
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            weightMap = new MultiMap<double>();
            draw = draw.And((arr, x, y) =>
            {
                double weight = weightFunc(arr, x, y);
                if (weight > double.MinValue)
                {
                    weightMap[x, y] = weight;
                }
                return true;
            });
        }
        #endregion
        Level.DrawBreadthFirstFill(Self.GridSpace.X, Self.GridSpace.Y, true, draw);
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            Log.printHeader("Flee Targets");
            MultiMap<GridType> gridMap = new MultiMap<GridType>();
            weightMap.DrawAll((arr, x, y) =>
            {
                arr.DrawRect(x, y, 5, Draw.AddGridTo<GridSpace, double>(Level, gridMap).And(Draw.FilterWalkable<double>(gridMap)));
                return true;
            });
            double max = weightMap.Max(x => x.val);
            double min = weightMap.Min(x => x.val);
            weightMap.DrawAll(Draw.TranslateRatio(gridMap, max, min));
            if (target != null)
            {
                gridMap[target] = GridType.INTERNAL_RESERVED_CUR;
            }
            foreach (NPCMemoryItem npcMem in filteredNPCMem)
            {
                gridMap[npcMem.NPC.GridSpace] = npcMem.Friendly ? GridType.INTERNAL_MARKER_FRIENDLY : GridType.Enemy;
            }
            gridMap[Self.GridSpace] = GridType.INTERNAL_MARKER_1;
            gridMap.ToLog(Log, "Move away area");
            Log.printFooter("Flee Targets");
        }
        #endregion
        if (target != null)
        {
            MoveTo(target);
        }
    }
    #endregion

    #region XML
    public void ParseXML(XMLNode x)
    {
        foreach (var packageNode in x.SelectList("AIDecision"))
        {
            AIDecision decision;
            if (ParseDecision(packageNode, out decision))
            {
                AddDecision(decision);
            }
        }

        // Package Defaults
        if (x.SelectBool("UseDefaults", true))
        {
            AddDecision(new AIMove());
            AddDecision(new AIWait());
            AddDecision(new AIUseAbility());
        }
    }

    protected void AddDecision(AIDecision decision)
    {
        foreach (AIState state in decision.States)
        {
            cores[(int)state].AddDecision(decision);
        }
        DecisionSet.Add(decision);
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

    public void ToLog(Log log)
    {
        if (!BigBoss.Debug.Flag(DebugManager.DebugFlag.AI)) return;
        log.w("Num Friendlies: " + NumFriendlies);
        log.w("Num Enemies: " + NumEnemies);
        log.w("Closest Enemy: " + ClosestEnemy + "  Dist: " + ClosestEnemyDist);
    }
}

public static class AIDecisions
{
    public static DecisionActions Then(this DecisionActions actions, DecisionActions rhs)
    {
        return (core) =>
        {
            actions(core);
            rhs(core);
        };
    }

    public static DecisionActions Base()
    {
        return (core) => { };
    }
}