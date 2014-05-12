using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Pathfinding;

public class NPC : Affectable
{
    public override string Prefab { get { return base.Prefab; } set { base.Prefab = "NPCs/" + value; } }
    /**
     * This method should be phased out down the line, it is used for temporary
     * data that needs initialized while other information is not known.
     */
    public override void Init()
    {
        TurnNPCIsOn = BigBoss.Time.CurrentTurn;
        //calculate any stats that need set on start
        CalcInitialStats();

        //get current grid position
        if (IsActive)
        {
            Vector2 currentPos = new Vector2(GO.transform.position.x.Round(), GO.transform.position.z.Round());
            GridSpace = BigBoss.Levels.Level[currentPos.x.ToInt(), currentPos.y.ToInt()];
        }

        //conversion of leveled items => items
        addToInventory(StartingItems.GetItems(Stats.Level));
    }

    #region NPC Properties
    [Copyable]
    public GenericFlags<NPCFlags> Flags = new GenericFlags<NPCFlags>();
    [Copyable]
    public GenericFlags<SpawnKeywords> SpawnKeywords = new GenericFlags<SpawnKeywords>();
    [Copyable]
    public AttributesData Attributes = new AttributesData();
    [Copyable]
    public Stats Stats = new Stats();
    [Copyable]
    public Spells KnownSpells = new Spells();
    [Copyable]
    public StartingItems StartingItems = new StartingItems();
    [Copyable]
    public AICore AI;

    public Inventory Inventory = new Inventory();
    protected List<Item> EquippedItems = new List<Item>();
    public Equipment Equipment = new Equipment();
    public Item NaturalWeapon { get; set; }
    public Spell OnDeath { get; set; }

    // Temporary arbitrary offset
    public Vector3 EyeSightPosition
    {
        get
        {
            Vector3 pos = this.GO.transform.position;
            return new Vector3(pos.x, pos.y + 2, pos.z);
        }
    }
    public override Vector3 CanSeePosition { get { return this.EyeSightPosition; } }
    #endregion

    #region Individual NPC Variables

    private ulong npcPoints = 0;
    private ulong TurnNPCIsOn = 1;
    EquipBones Bones = null;
    internal float turnTime;
    internal float gridTime;
    float gravity;
    internal Vector3 target;
    internal ActionToDo action;
    float TurnInterval = BigBoss.Time.TimeInterval;
    bool canMove;
    #endregion

    #region NPC Movement Properties

    public float Speed = 1.75f;  //temporarily hard-coded
    public float NPCSpeed { get { return Speed; } }

    internal bool Moving { get { return CurrentPath != null && !Acting; } } //stores moving condition
    internal bool Acting { get { return action != null && !action.IsDone(); } } //is the NPC already doing an action?
    protected float verticalOffset = 0f;
    internal Queue<GridSpace> targetGrids = new Queue<GridSpace>();

    internal Animator animator;
    internal CharacterController controller;
    internal Seeker seeker;
    internal float velocity;

    internal PFPath CurrentPath;
    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 1.2f;
    //The waypoint we are currently moving towards
    private int currentWaypoint = 0;

    #endregion

    public NPC()
        : base()
    {
        Stats.MaxEncumbrance = getMaxInventoryWeight();
        Stats.CurrentHealth = Stats.MaxHealth;
        Stats.CurrentPower = Stats.MaxPower;
        Stats.CurrentXP = 0;
        Stats.hungerRate = 1;
        AI = new AICore(this);
    }

    public override void Start()
    {
        animator = GO.GetComponent<Animator>() as Animator;
        controller = GO.GetComponent<CharacterController>();
        seeker = GO.GetComponent<Seeker>();
    }

    public override void Update()
    {
        if (IsActive)
        {
            float curTime = Time.time;
            if (curTime > turnTime)
            {
                turnTime = curTime + TurnInterval;
                if (subtractPoints(1))
                {
                    if (Acting)
                    {
                        canMove = false;
                        DoingAction();
                    }
                    else
                    {
                        //timeToMove += BigBoss.Time.TimeInterval;
                        canMove = true;
                        action = null;
                    }
                }
                else
                {
                    canMove = false;
                    velocity = 0;
                }
            }
            if (curTime > gridTime)
            {
                gridTime = curTime + .2f;
                UpdateCurrentTileVectors();
            }
            GetMovement();
        }
    }

    public virtual void DoingAction()
    {
        if (Acting)
        {
            action.Do();
        }
    }

    public override void FixedUpdate()
    {
        if (!Moving)
        {
            velocity = 0;
        }
        animator.SetFloat("runSpeed", velocity);
    }

    public virtual void Do(Action action, int cost, bool interuptible, bool actOnStart)
    {
        if (!Acting || this.action.Replaceable())
        {
            CurrentPath = null; //stops moving
            this.action = new ActionToDo(action, cost, interuptible, actOnStart);
        }
    }

    internal class ActionToDo
    {
        public Action action;
        public int turnsRemaining;
        public bool interuptible;
        public bool actOnStart;

        public ActionToDo(Action action, int turns, bool interupt, bool actOnStart)
        {
            this.action = action;
            this.turnsRemaining = turns;
            this.interuptible = interupt;
            this.actOnStart = actOnStart;

            if (actOnStart)
            {
                action();
            }
        }

        public void Do()
        {
            turnsRemaining--;
            if (turnsRemaining <= 0)
            {
                if (!actOnStart)
                {
                    action();
                }
            }
        }

        public bool IsDone()
        {
            return turnsRemaining <= 0;
        }

        public bool Replaceable()
        {
            return interuptible;
        }
    }

    public void CreateTextMessage(string str)
    {
        CreateTextMessage(str, Color.white);
    }

    public void CreateTextMessage(string str, Color col)
    {
        BigBoss.Gooey.CreateTextMessage(str, col);
    }

    public override void Wrap()
    {
        this.Instance = BigBoss.Objects.NPCs.Wrap(this, GridSpace);
    }

    #region Stats
    public bool get(NPCFlags fl)
    {
        return Flags[fl];
    }

    public void set(NPCFlags fl, bool on)
    {
        Flags[fl] = on;
    }

    public virtual float AdjustHunger(float amount)
    {
        Stats.Hunger += amount;
        CreateTextMessage("Gained " + amount + " of nutrition.");
        getHungerLevel(Stats.Hunger);
        return Stats.Hunger;
    }

    public virtual void AddLevel()
    {
        Stats.Level++;
        //do level up stuff here
        CalcStats();
    }

    public virtual bool AdjustHealth(int amount, bool report = true) //returns true if NPC dies from health gain
    {
        if (amount <= 0) //if amount is < 0 it's damage
        {
            amount = -amount; //flips it for below calculations to make sense as a subtraction of health
            if (Stats.CurrentHealth - amount > 0)
            {
                Stats.CurrentHealth = Stats.CurrentHealth - amount;
                if (report) CreateTextMessage(this.Name + " took " + amount + " damage!", Color.red);
                return false;
            }
            else
            {
                this.killThisNPC(); //NPC is now dead!
                return true;
            }
        }
        //health is positive gain here, but gain is more than max health... so we cap it
        else if (Stats.CurrentHealth + amount > Stats.MaxHealth)
        {
            Stats.CurrentHealth = Stats.MaxHealth;
            if (report) CreateTextMessage(this.Name + " gained " + amount + " in health.");
            return false;
        }
        else // health is positive gain but doesn't go over max, so it works normally
        {
            Stats.CurrentHealth = Stats.CurrentHealth + amount;
            if (report) CreateTextMessage(this.Name + " gained " + amount + " in health.");
            return false;
        }
    }

    public virtual bool SetHealth(int amount)
    {
        if (amount > Stats.MaxHealth)
        {
            return SetHealth(Stats.MaxHealth);
        }
        amount = amount - Stats.CurrentHealth;
        return AdjustHealth(amount, false);
    }

    public virtual void AdjustPower(int amount)
    {
        if (amount <= 0)
        {
            amount = -amount;
            if (Stats.CurrentPower - amount > 0)
            {
                Stats.CurrentPower = Stats.CurrentPower - amount;
            }
            else
            {
                this.Stats.CurrentPower = 0;
            }
        }
        else if (Stats.CurrentPower + amount > Stats.MaxPower)
        {
            Stats.CurrentPower = Stats.MaxPower;
        }
        else
        {
            Stats.CurrentPower = Stats.CurrentPower + amount;
        }
    }

    public virtual bool damage(int amount) //returns positive if NPC dies
    {
        //this 'damages' for a positive number, sending it as a negative adjustment
        return AdjustHealth(-amount);
    }

    public virtual bool AdjustMaxHealth(int amount)
    {
        Stats.MaxHealth += amount;
        if (Stats.MaxHealth <= 0)
        {
            killThisNPC();
            return true;
        }
        if (this.IsNotAFreaking<Player>()) SetHealth(Stats.MaxHealth);
        return false;
    }

    public virtual bool SetMaxHealth(int amount)
    {
        amount = amount - Stats.MaxHealth;
        return AdjustMaxHealth(amount);
    }

    public virtual void AdjustMaxPower(int amount)
    {
        Stats.MaxPower += amount;
    }

    public virtual void AdjustXP(float amount)
    {
        Stats.CurrentXP += amount;
        if (Stats.CurrentXP > calcXPForNextLevel())
        {
            AddLevel();
        }
    }

    public virtual void AdjustAttribute(Attributes attr, int amount)
    {
        Attributes.set(attr, Attributes.get(attr) + amount);
        CalcStats();
    }
    #endregion

    #region Stat Calculations
    //Use this for a re-calc on level up or any attribute changes.
    protected void CalcStats()
    {
        //need to define level changes to stats, percentages on NPC's, maybe?
        NPC proto = BigBoss.Objects.NPCs.GetPrototype(this.Name);
        if (proto != null)
        {
            SetMaxHealth((int)(Stats.Level * Attributes.Constitution * .01f + proto.Stats.MaxHealth));
        }
        else
        {
            SetMaxHealth((int)(50 + Stats.Level * Attributes.Constitution * .2f));
        }

        //these need adjusted to virtual functions so Player can update GUI
        Stats.MaxEncumbrance = getMaxInventoryWeight();
    }

    protected void CalcInitialStats()
    {
        CalcStats();
        AdjustHealth(Stats.MaxHealth - Stats.CurrentHealth, false);
        Stats.CurrentPower = Stats.MaxPower;
        Stats.CurrentXP = 0;
        Stats.hungerRate = 1;
    }

    protected float calcXPForNextLevel()
    {
        //do calc here
        return (float)Math.Pow(Stats.Level, 1.2) * 15;
        //return (100 + ((Mathf.Pow(Stats.Level, 2f) / 2)));
    }

    protected void getHungerLevel(float hunger)
    {
        HungerLevel prior = Stats.HungerLevel;
        // These numbers don't make sense.
        if (hunger < 50)
            Stats.HungerLevel = HungerLevel.Faint;
        else if (hunger < 130)
            Stats.HungerLevel = HungerLevel.Starving;
        else if (hunger < 500)
            Stats.HungerLevel = HungerLevel.Hungry;
        else if (hunger < 800)
            Stats.HungerLevel = HungerLevel.Satiated;
        else if (hunger < 1000)
            Stats.HungerLevel = HungerLevel.Stuffed;
        if (prior != Stats.HungerLevel)
        {
            BigBoss.Gooey.UpdateHungerLevel(Stats.HungerLevel);
        }
    }

    public float getXPfromNPC()
    {
        //how much XP is this NPC worth?
        return this.Stats.Level * 5;
    }
    #endregion

    #region Checks
    public bool CanSee(WorldObject obj)
    {
        if (this.Instance == null || obj.Instance == null)
        {
            return false;
        }
        return !Physics.Linecast(this.EyeSightPosition, obj.CanSeePosition);
    }
    #endregion

    #region Movement

    public virtual bool UpdateCurrentTileVectors() //assigns gridspaces every interval
    {
        Vector2 currentLoc = new Vector2(GO.transform.position.x.Round(), GO.transform.position.z.Round());
        if (BigBoss.Levels.Level == null) return false;
        GridSpace newGridSpace = BigBoss.Levels.Level[currentLoc.x.ToInt(), currentLoc.y.ToInt()];
        if (newGridSpace != null && !newGridSpace.IsBlocked() && GridTypeEnum.Walkable(newGridSpace.Type))
        {
            GridSpace = newGridSpace;
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void GetMovement() //only for NPC's
    {
        if (!canMove) return;
        if (CurrentPath == null) return; //no path, don't move
        if (!CurrentPath.IsDone()) return; //path isn't done (should never occur)
        if (!CurrentPath.foundEnd || currentWaypoint >= CurrentPath.vectorPath.Length) //path has no end, or at the end of path
        {
            CurrentPath = null;
            return; //path finished, set to null
        }
        //Direction to the next waypoint
        Vector3 dir = (CurrentPath.vectorPath[currentWaypoint]);
        LookTowards(dir); //orient towards it
        MoveForward(); //move forward
        velocity = NPCSpeed; //sets animation velocity

        if ((CurrentPath.vectorPath[currentWaypoint] - GO.transform.position).sqrMagnitude < nextWaypointDistance)
        { //if the waypoint is close, transition to the next waypoint
            currentWaypoint++; //if they get stuck, trying to back turn - next pathing call would fix
            return;
        }
    }

    internal void MoveForward() //mainly for NPC's
    {
        Vector3 moveDir = GO.transform.TransformDirection(Vector3.forward);
        if (controller.isGrounded)
        {
            gravity = 0;
        }
        else { gravity -= 9.81f * Time.deltaTime; }
        Vector3 newMove = new Vector3(moveDir.x, gravity, moveDir.z);
        controller.Move(newMove * NPCSpeed * Time.deltaTime);
    }

    internal void MoveForward(float speed) //mainly for player, because he has variable movement
    {
        Vector3 moveDir = GO.transform.TransformDirection(Vector3.forward);
        if (controller.isGrounded)
        {
            gravity = 0;
        }
        else { gravity -= 9.81f * Time.deltaTime; }
        Vector3 newMove = new Vector3(moveDir.x, gravity, moveDir.z);
        controller.Move(newMove * speed * Time.deltaTime);
    }

    internal void LookTowards(Vector3 target) //lerp orientation towards target
    {
        Vector3 heading = new Vector3(target.x - GO.transform.position.x, 0f, target.z - GO.transform.position.z);
        Quaternion lerp = Quaternion.LookRotation(heading);
        Quaternion toRot = Quaternion.Lerp(GO.transform.rotation, lerp, 3 * NPCSpeed * Time.deltaTime);
        GO.transform.rotation = toRot;
    }

    void OnPathComplete(PFPath p)
    {
        if (!p.error)
        {
            CurrentPath = p;
            //Reset the waypoint counter
            currentWaypoint = 1;
        }
    }

    public void MoveNPC(GridSpace node)
    {
        MoveNPC(new Vector3(node.X, 0, node.Y));
    }

    public void MoveNPC(Vector3 pos)
    {
        if (target == Vector3.zero) target = pos; //if target position is close to last target, don't bother w/ new path
        if (((pos - target).sqrMagnitude > .3f) || CurrentPath == null)
        {
            target = pos;
            seeker.StartPath(GO.transform.position, pos, OnPathComplete);
        }
    }

    internal virtual void SetAttackAnimation(GameObject target)
    {
        float testVal = UnityEngine.Random.value;
        GO.transform.LookAt(target.transform);
        if (testVal < .333)
        {
            animator.Play(Equipment.WeaponAnims.Attack1);
        }
        else if (testVal < .666)
        {
            animator.Play(Equipment.WeaponAnims.Attack2);
        }
        else
        {
            animator.Play(Equipment.WeaponAnims.Attack3);
        }
    }
    #endregion

    #region Actions

    public virtual void eatItem(Item i)
    {
        Do(new Action(() =>
        {
            //enforces it being in inventory, if that should change we'll rewrite later
            if (Inventory.Contains(i))
            {
                //item was just eaten, take it outta that list
                if (i.itemFlags[ItemFlags.IS_EQUIPPED])
                {
                    unequipItem(i);
                }
                i.onEatenEvent(this);
            }
        }), BigBoss.Time.eatItemCost, false, true);
    }

    public virtual void useItem(Item i)
    {
        Do(new Action(() =>
        {
            if (i.isUsable())
            {
                i.onUseEvent(this);
            }
        }), BigBoss.Time.useItemCost, false, true);
    }

    public virtual void attack(NPC n)
    {
        Do(new Action(() =>
        {
            List<Item> weapons = Equipment.GetWeapons();
            if (weapons.Count > 0)
            {
                foreach (Item i in weapons)
                {
                    CreateTextMessage("The " + this.Name + " swings with his " + i.Name + "!");
                    SetAttackAnimation(n.GO);
                    if (i.Damage(n))
                    {
                        AdjustXP(n.getXPfromNPC());
                    }
                }
            }
            else
            {
                CreateTextMessage("The " + this.Name + " swings with his bare hands!");
                SetAttackAnimation(n.GO);
                if (NaturalDamage(n))
                {
                    AdjustXP(n.getXPfromNPC());
                }
            }
        }), BigBoss.Time.attackCost, false, true);
    }

    public virtual void CastSpell(string spell, params IAffectable[] targets)
    {
        CastSpell(KnownSpells[spell], targets);
    }

    public virtual void CastSpell(Spell spell, params IAffectable[] targets)
    {
        Do(new Action(() =>
        {
            spell.Activate(this, targets);
            AdjustPower(-spell.cost);
        }), BigBoss.Time.spellCost, true, false);
    }

    protected bool NaturalDamage(NPC n)
    {
        if (NaturalWeapon != null)
        {
            return NaturalWeapon.Damage(n);
        }
        else
            return n.damage(CalcHandDamage());
    }

    protected int CalcHandDamage()
    {
        return (new System.Random()).Next(0, Attributes.Strength);
    }

    private void killThisNPC()
    {
        //do all the calculations/etc here
        //drop the items here
        //etc etc

        if (this.IsNotAFreaking<Player>())
        {
            CreateTextMessage(Name + " is dead!", Color.red);
            Debug.Log(this.Name + " was killed!");
            //time effects need cleared
            List<Item> itemsToDrop = new List<Item>();
            foreach (InventoryCategory ic in Inventory.Values)
            {
                foreach (Item i in ic.Values)
                {
                    Debug.Log("Adding to drop list: " + i.Name);
                    itemsToDrop.Add(i);
                }
            }
            foreach (Item i in itemsToDrop)
            {
                Debug.Log("Dropping item: " + i.Name);
                this.dropItem(i, GridSpace);
            }
            OnDeath.Activate(this);
        }
        else
        {
            Debug.Log("Player is dead! Uhh, what do we do now?");
        }
    }
    #endregion

    #region Inventory, Items, and Equipment
    public void addToInventory(Item item)
    {
        this.addToInventory(item, 1);
    }

    public void addToInventory(List<Item> items)
    {
        if (items.Count > 0)
        {
            foreach (Item item in items)
            {
                this.addToInventory(item, 1);
            }
        }
    }

    public virtual void addToInventory(Item item, int count)
    {
        Inventory.Add(item, count);
    }

    public void removeFromInventory(Item item)
    {
        removeFromInventory(item, 1);

    }

    public virtual void removeFromInventory(Item item, int count)
    {
        if (Inventory.Contains(item))
        {
            Inventory.Remove(item, count);
        }
        else
        {
            //do nothing, the item isn't there
        }
    }

    public float getMaxInventoryWeight()//Should only be calc'd when weight changes or attribute on player is affected
    {
        float invWeightMax;
        //Add formula here
        invWeightMax = (25 * (Attributes.Strength + Attributes.Constitution) + 50);
        return invWeightMax;
    }


    private EquipBones GetEquipBones()
    {
        if (GO != null && Bones == null)
        {
            Bones = GO.GetComponent<EquipBones>();
        }
        return Bones;
    }

    public virtual bool equipItem(Item i)
    {
        if (Equipment.equipItem(i, GetEquipBones()))
        {
            i.onEquipEvent(this);
            EquippedItems.Add(i);
            if (Equipment.WeaponAnims.Move != "") animator.SetBool(Equipment.WeaponAnims.Move, true);
            Wait(BigBoss.Time.equipItemCost);
            return true;
        }
        return false;
    }

    public virtual bool unequipItem(Item i)
    {
        if (i.isUnEquippable() && Equipment.removeItem(i, animator))
        {
            i.onUnEquipEvent(this);
            if (EquippedItems.Contains(i))
            {
                EquippedItems.Remove(i);
            }
            Wait(BigBoss.Time.equipItemCost);
            return true;
        }
        return false;
    }

    public List<Item> getEquippedItems()
    {
        return EquippedItems;
    }

    internal bool dropItem(Item i, GridSpace space)
    {
        bool var = Inventory.TransferTo(i, space);
        if (var) Wait(BigBoss.Time.pickDropItemCost);
        return var;
    }

    internal bool pickUpItem(Item i, GridSpace space)
    {
        bool var = Inventory.TransferFrom(i, space);
        if (var) Wait(BigBoss.Time.pickDropItemCost);
        return var;
    }

    public void Wait()
    {
    }

    public void Wait(int turns)
    {
        Do(Wait, turns, false, true);
    }

    #endregion

    #region XML
    public override void ParseXML(XMLNode x)
    {
        base.ParseXML(x);
        Attributes = x.Select<AttributesData>("attributes");
        Stats = x.Select<Stats>("stats");
        Flags = new GenericFlags<NPCFlags>(x.SelectEnums<NPCFlags>("flags"));
        SpawnKeywords = new GenericFlags<SpawnKeywords>(x.SelectEnums<SpawnKeywords>("spawnkeywords"));
        KnownSpells = x.Select<Spells>("spells");
        StartingItems = x.Select<StartingItems>("startingitems");
        Equipment = x.Select<Equipment>("equipslots");
        NaturalWeapon = x.Select<Item>("naturalweapon");
        OnDeath = x.Select<Spell>("OnDeath");
        x.Select("AI", AI);
    }
    #endregion

    #region Turn Management
    public bool subtractPoints(ulong points)
    {
        ulong gameTurns = BigBoss.Time.CurrentTurn;
        if (TurnNPCIsOn < gameTurns)
        {
            npcPoints += (gameTurns - TurnNPCIsOn);
            TurnNPCIsOn = gameTurns;
        }

        if (npcPoints >= points)
        {
            npcPoints -= points;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void UpdateTurn()
    {
        if (IsActive)
        {
            AI.DecideWhatToDo();
        }
    }
    #endregion

    #region AI

    public bool IsNextToGrid(GridSpace targetSpace, bool includeSelfSpace)
    {
        Value2D<GridSpace> space;
        if (includeSelfSpace && this.GridSpace.Equals(targetSpace)) return true;
        return BigBoss.Levels.Level.Array.GetPointAround(targetSpace.X, targetSpace.Y, true, (arr, x, y) =>
        {
            return this.GridSpace.X == x && this.GridSpace.Y == y;
        }, out space);
    }

    public bool IsNextToTarget(WorldObject n)
    {
        if (GO != null)
            if (Vector3.Distance(GO.transform.position, n.GO.transform.position) < 1.75f)
            {
                return true;
            }
        return false;
    }

    public float DistanceToTarget(NPC n)
    {
        if (GO != null)
            return Vector3.Distance(GO.transform.position, n.GO.transform.position);
        else return float.MaxValue;
    }

    public float DistanceToTarget(GameObject obj)
    {
        if (GO != null)
            return Vector3.Distance(GO.transform.position, obj.transform.position);
        else return float.MaxValue;
    }

    public bool HasDecision(AIDecision decision)
    {
        return AI.Contains(decision);
    }

    public bool InCombat()
    {
        return AI.CurrentState == AIState.Combat;
    }
    #endregion

    #region Touch Input
    public override void OnClick()
    {
        if (BigBoss.PlayerInput.InputSetting[InputSettings.DEFAULT_INPUT])
        {
            if (this.IsNotAFreaking<Player>())
            {
                if (this.IsNextToTarget(BigBoss.Player))
                {
                    BigBoss.Player.attack(this);
                }
            }
        }
        else if (BigBoss.PlayerInput.InputSetting[InputSettings.SPELL_INPUT])
        {
            if (this.DistanceToTarget(BigBoss.Player) > BigBoss.Gooey.GetCurrentSpellRange())
            {
                CreateTextMessage(this.Name + " is too far away to cast this spell!");
            }
            else
            {
                BigBoss.Gooey.Target(this);
            }
        }
    }
    #endregion
}
