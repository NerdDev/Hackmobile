using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using XML;

public class NPC : Affectable
{
    public override string Prefab { get { return base.Prefab; } set { base.Prefab = "NPCs/" + value; } }
    /**
     * This method should be phased out down the line, it is used for temporary
     * data that needs initialized while other information is not known.
     */
    public override void Init()
    {
        //initalize AI
        Master.InitAI(this);

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

    /**
     * All the properties of the NPC should be contained here.
     */
    #region NPC Properties
    public GenericFlags<NPCFlags> Flags = new GenericFlags<NPCFlags>();
    public GenericFlags<SpawnKeywords> SpawnKeywords = new GenericFlags<SpawnKeywords>();
    public AttributesData Attributes = new AttributesData();
    public Stats Stats = new Stats();
    public Spells KnownSpells = new Spells();
    public StartingItems StartingItems = new StartingItems();
    private AICore Master = new AICore();

    public Inventory Inventory = new Inventory();
    protected List<Item> EquippedItems = new List<Item>();
    public Equipment Equipment = new Equipment();
    public Item NaturalWeapon { get; set; }
    #endregion

    /**
     * Anything relating to movement.
     */
    #region NPC Movement Properties

    public float Speed = 1.75f;  //temporarily hard-coded
    public float NPCSpeed { get { return Speed; } }

    private float _rotationSpeed = .5f;  //temporarily hard-coded
    public float NPCRotationSpeed { get { return _rotationSpeed; } }

    //public Vector3 CurrentOccupiedGridCenterWorldPoint;
    //public Vector3 LastOccupiedGridCenterWorldPoint;

    internal bool moving; //stores moving condition
    protected bool verticalMoving;
    protected bool movingUp;
    protected float verticalOffset = 0f;
    internal Queue<GridSpace> targetGrids = new Queue<GridSpace>();
    internal Vector3 heading; //this is the heading of target minus current location

    internal Animator animator;
    float velocity;

    static int idleState = Animator.StringToHash("Base Layer.idle");
    static int moveState = Animator.StringToHash("Base Layer.Locomotion_Tree");
    static public int attackState1 = Animator.StringToHash("Base Layer.attack 1");
    static public int attackState2 = Animator.StringToHash("Base Layer.attack 2");
    static int attackState3 = Animator.StringToHash("Base Layer.attack 3");
    static int deathState = Animator.StringToHash("Base Layer.death");

    #endregion

    public NPC()
    {
        Stats.MaxEncumbrance = getMaxInventoryWeight();
        Stats.CurrentHealth = Stats.MaxHealth;
        Stats.CurrentPower = Stats.MaxPower;
        Stats.CurrentXP = 0;
        Stats.hungerRate = 1;
    }

    public override void Start()
    {
        animator = GO.GetComponent<Animator>() as Animator;
    }

    public override void Update()
    {
        if (IsActive)
        {
            if (moving)
            {
                movement();
            }
            if (verticalMoving)
            {
                verticalMovement();
            }
        }
    }

    public override void FixedUpdate()
    {
        if (moving)
        {
            if (velocity < NPCSpeed)
            {
                velocity += .01f;
            }
            else
            {
                velocity = NPCSpeed;
            }
        }
        else
        {
            velocity = 0;
        }
        if (animator == null)
        {
            animator = GO.GetComponent<Animator>() as Animator;
        }
        else
        {
            animator.SetFloat("runSpeed", velocity);
        }
    }

    internal void SetAttackAnimation(GameObject target)
    {
        float testVal = UnityEngine.Random.value;
        GO.transform.LookAt(target.transform);
        if (this is Player)
        {
            //GO.transform.Rotate(Vector3.up, 45f);
        }
        if (testVal < .333)
        {
            animator.Play(attackState1);
        }
        else if (testVal < .666)
        {
            animator.Play(attackState2);
        }
        else
        {
            animator.Play(attackState3);
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
        return Stats.Level * 15;
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
        return 10f;
    }
    #endregion

    #region Movement
    private void movement()
    {
        GridSpace currentTarget = targetGrids.Peek();
        if (!checkXYPosition(GO.transform.position, new Vector3(currentTarget.X, 0f, currentTarget.Y)))
        {
            MoveNPCStepwise(currentTarget);
        }
        else
        {
            GridSpace grid = targetGrids.Dequeue();
            GO.transform.position = new Vector3(grid.X, verticalOffset, grid.Y);

            if (targetGrids.Count <= 0)
            {
                moving = false;
            }
        }
    }

    protected void verticalMovement()
    {
        if (!checkVerticalPosition(GO.transform.position, new Vector3(0f, verticalOffset, 0f)))
        {
            MoveNPCStepwiseUp();
        }
        else
        {
            verticalMoving = false;
            Vector3 pos = GO.transform.position;
            pos.y = verticalOffset;
            GO.transform.position = pos;
        }
    }

    public void MoveNPC(GridSpace node)
    {
        //GridSpace grid = BigBoss.Levels.Level[node.X, node.Y];
        if (!node.IsBlocked() && subtractPoints(BigBoss.Time.regularMoveCost))
        {
            GridSpace = node;
            move(node);
        }
    }

    public void move(GridSpace node)
    {
        moving = true;
        targetGrids.Enqueue(node);
    }

    public void verticalMove(float z)
    {
        verticalMoving = true;
        verticalOffset += z;
        if (z > 0)
        {
            movingUp = true;
        }
        else
        {
            movingUp = false;
        }
    }

    internal void MoveNPCStepwise(GridSpace gridTarget)
    {
        GO.MoveStepWise(new Vector3(gridTarget.X, 0, gridTarget.Y), NPCSpeed);
        //heading = new Vector3(gridTarget.X - GO.transform.position.x, 0f, gridTarget.Y - GO.transform.position.z);
        //GO.transform.Translate(Vector3.forward * NPCSpeed * Time.deltaTime, Space.Self);
        //Quaternion toRot = Quaternion.LookRotation(heading);
        //GO.transform.rotation = toRot;
        //GO.transform.rotation = Quaternion.Slerp(GO.transform.rotation, toRot, NPCRotationSpeed);
    }

    void MoveNPCStepwiseUp()
    {
        if (movingUp)
        {
            GO.transform.Translate(Vector3.up * NPCSpeed * Time.deltaTime, Space.Self);
        }
        else
        {
            GO.transform.Translate(Vector3.down * NPCSpeed * Time.deltaTime, Space.Self);
        }
    }

    protected float variance = .08f;
    protected bool checkXYPosition(Vector3 playPos, Vector3 curPos)
    {
        if (Math.Abs(playPos.x - curPos.x) > variance ||
            Math.Abs(playPos.z - curPos.z) > variance)
        {
            return false;
        }
        return true;
    }

    protected bool checkVerticalPosition(Vector3 playPos, Vector3 curPos)
    {
        if (Math.Abs(playPos.y - curPos.y) > variance)
        {
            return false;
        }
        return true;
    }

    //public PathTree getPathTree(GridSpace dest)
    //{
    //    PathTree path = new PathTree(GridSpace, dest);
    //    return path;
    //}
    #endregion

    #region Actions

    public virtual void eatItem(Item i)
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
    }

    public virtual void useItem(Item i)
    {
        if (i.isUsable())
        {
            i.onUseEvent(this);
        }
    }

    public virtual void attack(NPC n)
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
    }

    public virtual void CastSpell(string spell, params IAffectable[] targets)
    {
        CastSpell(KnownSpells[spell], targets);
    }

    public virtual void CastSpell(Spell spell, params IAffectable[] targets)
    {
        spell.Activate(this, targets);
        AdjustPower(-spell.cost);
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
            JustUnregister();
            animator.Play(deathState);
            GO.AddComponent<TimedAction>().init(1.5f, new Action(() => { JustDestroy(); }));
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

    public virtual bool equipItem(Item i)
    {
        if (Equipment.equipItem(i))
        {
            i.onEquipEvent(this);
            EquippedItems.Add(i);
            return true;
        }
        return false;
    }

    public virtual bool unequipItem(Item i)
    {
        if (i.isUnEquippable() && Equipment.removeItem(i))
        {
            i.onUnEquipEvent(this);
            if (EquippedItems.Contains(i))
            {
                EquippedItems.Remove(i);
            }
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
        return Inventory.TransferTo(i, space);
    }

    internal bool pickUpItem(Item i, GridSpace space)
    {
        return Inventory.TransferFrom(i, space);
    }


    #endregion

    #region NPC Data Management for Instances
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
        //parse AI packages
    }
    #endregion

    #region Turn Management

    private int npcPoints = 0;
    private int baseNPCPoints = 1;

    private bool subtractPoints(int points)
    {
        if (npcPoints > points)
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
            try
            {
                if (this.IsNotAFreaking<Player>())
                {
                    Master.DecideWhatToDo();
                }
            }
            catch (Exception e)
            {
                Debug.Log("Exception: ");
                Debug.Log(e.ToString());
            }
        }
    }

    public override int CurrentPoints
    {
        get
        {
            return this.npcPoints;
        }
        set
        {
            this.npcPoints = value;
        }
    }

    public override int BasePoints
    {
        get
        {
            return this.baseNPCPoints;
        }
        set
        {
            this.baseNPCPoints = value;
        }
    }

    public override bool IsActive
    {
        get
        {
            return this.isActive;
        }
        set
        {
            this.isActive = value;
        }
    }
    #endregion

    #region AI

    public bool IsNextToTarget(GridSpace targetSpace)
    {
        Value2D<GridSpace> space;
        return BigBoss.Levels.Level.Array.GetPointAround(targetSpace.X, targetSpace.Y, true, (arr, x, y) =>
        {
            return this.GridSpace.X == x && this.GridSpace.Y == y;
        }, out space);
    }

    public bool IsNextToTarget(NPC n)
    {
        GridSpace targetSpace = n.GridSpace;
        return IsNextToTarget(targetSpace);
    }

    public int GridDistanceToTarget(NPC n)
    {
        GridSpace targetSpace = n.GridSpace;
        PathNode[] nodes = PathTree.Instance.getPath(this.GridSpace, targetSpace, 50).ToArray();
        return nodes.Length;
    }

    #endregion

    #region Touch Input
    public override void OnClick()
    {
        if (BigBoss.PlayerInput.defaultPlayerInput)
        {
            if (this.IsNotAFreaking<Player>())
            {
                if (this.IsNextToTarget(BigBoss.Player))
                {
                    BigBoss.Player.attack(this);
                }
            }
        }
        else if (BigBoss.PlayerInput.spellInput)
        {
            if (this.GridDistanceToTarget(BigBoss.Player) > BigBoss.Gooey.GetCurrentSpellRange())
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
