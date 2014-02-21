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
        calcStats();
        Equipment = new Equipment(this.Bodyparts);
        //IsActive = true;
        if (IsActive)
        {
            Vector2 currentPos = new Vector2(GO.transform.position.x.Round(), GO.transform.position.z.Round());
            GridSpace = BigBoss.Levels.Level[currentPos.x.ToInt(), currentPos.y.ToInt()];
        }
        InitAI();
    }

    /**
     * All the properties of the NPC should be contained here.
     */
    #region NPC Properties
    public ESFlags<NPCFlags> Flags = new ESFlags<NPCFlags>();
    public ESFlags<Keywords> Keywords = new ESFlags<Keywords>();
    public Race Race;
    public Role Role;
    public AttributesData Attributes = new AttributesData();
    public BodyParts Bodyparts = new BodyParts();
    public Stats Stats = new Stats();
    public Dictionary<string, Spell> KnownSpells = new Dictionary<string, Spell>();

    //public List<Item> inventory = new List<Item>();
    public Inventory Inventory = new Inventory();
    protected List<Item> EquippedItems = new List<Item>();
    public Equipment Equipment = null;
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

    Animator animator;
    float velocity;

    #endregion

    public NPC()
    {
        Equipment = new Equipment(Bodyparts);
        Stats.MaxEncumbrance = getMaxInventoryWeight();
        Stats.CurrentHealth = Stats.MaxHealth;
        Stats.CurrentPower = Stats.MaxPower;
        Stats.XPToNextLevel = calcXPForNextLevel();
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

    public void CreateTextPop(string str)
    {
        BigBoss.Gooey.CreateTextPop(GO.transform.position, str);
    }

    public void CreateTextPop(string str, Color col)
    {
        BigBoss.Gooey.CreateTextPop(GO.transform.position, str, col);
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
        CreateTextPop("Gained " + amount + " of nutrition.");
        getHungerLevel(Stats.Hunger);
        return Stats.Hunger;
    }

    public virtual void AddLevel()
    {
        Stats.Level++;
        //do level up stuff here
        calcStats();
    }

    public virtual void AdjustHealth(int amount)
    {
        if (amount < 0)
        {
            damage(-amount);
        }
        else if (Stats.CurrentHealth + amount > Stats.MaxHealth)
        {
            Stats.CurrentHealth = Stats.MaxHealth;
            CreateTextPop(this.Name + " gained " + amount + " in health.");
        }
        else
        {
            Stats.CurrentHealth = Stats.CurrentHealth + amount;
            CreateTextPop(this.Name + " gained " + amount + " in health.");
        }
    }

    public virtual bool damage(int amount)
    {
        if (Stats.CurrentHealth - amount > 0)
        {
            Stats.CurrentHealth = Stats.CurrentHealth - amount;
            //Debug.Log(this.Name + " was damaged for " + amount + "!");
            CreateTextPop("Damaged for " + amount + "!", Color.red);
            return false;
        }
        else
        {
            this.killThisNPC(); //NPC is now dead!
            return true;
        }
    }

    public virtual void AdjustMaxHealth(int amount)
    {
        Stats.MaxHealth += amount;
    }

    public virtual void AdjustXP(float amount)
    {
        Stats.CurrentXP += amount;
        if (Stats.CurrentXP > Stats.XPToNextLevel)
        {
            AddLevel();
        }
    }

    public virtual void AdjustAttribute(Attributes attr, int amount)
    {
        Attributes.set(attr, Attributes.get(attr) + amount);
        calcStats();
    }
    #endregion

    #region Stat Calculations
    //Use this for a re-calc on level up or any attribute changes.
    protected void calcStats()
    {
        Stats.MaxEncumbrance = getMaxInventoryWeight();
        Stats.XPToNextLevel = calcXPForNextLevel();
    }

    protected float calcXPForNextLevel()
    {
        //do calc here
        return (100 + ((Mathf.Pow(Stats.Level, 3f) / 2)));
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
            int xmove = GridSpace.X - node.X;
            int ymove = GridSpace.Y - node.Y;
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
        if (Equipment.getItems(EquipTypes.HAND) != null)
        {
            List<Item> weapons = Equipment.getItems(EquipTypes.HAND);
            if (weapons.Count > 0)
            {
                foreach (Item i in weapons)
                {
                    CreateTextPop("The " + this.Name + " swings with his " + i.Name + "!");
                    if (!n.damage(i.getDamage()))
                    {
                    }
                    else
                    {
                        AdjustXP(n.getXPfromNPC());
                    }
                }
            }
            else
            {
                CreateTextPop("The " + this.Name + " swings with his bare hands!");
                if (!n.damage(calcHandDamage()))
                {
                }
                else
                {
                    AdjustXP(n.getXPfromNPC());
                }
            }
        }
        else
        {
            CreateTextPop("The " + this.Name + " swings with his bare hands!");
            //attacking with bare hands
            if (!n.damage(calcHandDamage()))
            {
            }
            else
            {
                AdjustXP(n.getXPfromNPC());
            }
        }
    }

    public virtual void CastSpell(string spell, params IAffectable[] targets)
    {
        KnownSpells[spell].Activate(this, targets);
    }

    public virtual void CastSpell(Spell spell, params IAffectable[] targets)
    {
        spell.Activate(this, targets);
    }

    protected int calcHandDamage()
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
            CreateTextPop(Name + " is dead!", Color.red);
            Debug.Log(this.Name + " was killed!");
            Destroy();
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

    public float getCurrentInventoryWeight()
    {
        float tempWeight = 0;
        //foreach (Item kvp in inventory)
        //{
        //    tempWeight += kvp.props.Weight;
        //}

        return tempWeight;
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
        Race = x.SelectEnum<Race>("race");
        Role = x.SelectEnum<Role>("role");
        Attributes = x.Select<AttributesData>("attributes");
        Bodyparts = x.Select<BodyParts>("bodyparts");
        Stats = x.Select<Stats>("stats");
        Flags = x.Select<ESFlags<NPCFlags>>("flags");
        Keywords = x.Select<ESFlags<Keywords>>("keywords");
        foreach (XMLNode spell in x.SelectList("spell"))
        {
            string spellName = spell.SelectString("name");
            Spell s = spell.Select<Spell>();
            KnownSpells.Add(spellName, s);
            if (this.Name.Equals("player"))
            {
                BigBoss.Objects.PlayerSpells.Add(spellName, s);
            }
        }
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
                    DecideWhatToDo();
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

    internal List<AIAction> actions = new List<AIAction>();

    void InitAI()
    {
        actions.Add(new Attack(this));
        actions.Add(new Move(this));
    }
    public abstract class AIAction : IComparable<AIAction>
    {
        private AIAction() { }
        public AIAction(NPC n)
        {
            this.npc = n;
        }
        internal NPC npc;
        public int Cost { get; set; }
        internal int Weight { get; set; }
        public abstract void Action();
        public abstract void CalcWeighting();
        public int CompareTo(AIAction other)
        {
            if (this.Weight < other.Weight)
            {
                return -1;
            }
            else if (this.Weight == other.Weight)
            {
                return 0;
            }
            return 1;
        }
    }
    internal class Attack : AIAction
    {
        public Attack(NPC n)
            : base(n)
        {
            Cost = 60;
        }

        public override void Action()
        {
            npc.attack(BigBoss.Player);
        }

        public override void CalcWeighting()
        {
            if (npc.IsNextToPlayer())
            {
                Weight = 50;
            }
            else
            {
                Weight = 0;
            }
        }
    }
    internal class Move : AIAction
    {
        public Move(NPC n)
            : base(n)
        {
            Cost = 60;
        }

        public override void Action()
        {
            PathNode[] nodes = PathTree.Instance.getPath(npc.GridSpace, BigBoss.Player.GridSpace, 75).ToArray();
            if (nodes.Length > 2)
            {
                GridSpace nodeToMove = nodes[nodes.Length - 2].loc;
                npc.MoveNPC(nodeToMove);
            }
        }

        public override void CalcWeighting()
        {
            if (npc.IsNextToPlayer())
            {
                Weight = 0;
            }
            else
            {
                Weight = 50;
            }
        }
    }

    bool IsNextToPlayer()
    {
        GridSpace playerSpace = BigBoss.Player.GridSpace;
        Value2D<GridSpace> space;
        return BigBoss.Levels.Level.Array.GetPointAround(playerSpace.X, playerSpace.Y, true, (arr, x, y) =>
        {
            return this.GridSpace.X == x && this.GridSpace.Y == y;
        }, out space);
    }

    void DecideWhatToDo()
    {
        foreach (AIAction action in actions)
        {
            action.CalcWeighting();
        }
        actions.Sort();
        if (actions[actions.Count - 1].Weight > 0)
        {
            actions[actions.Count - 1].Action();
        }
    }

    #endregion

    #region Touch Input
    public override void OnClick()
    {
        if (BigBoss.PlayerInput.defaultPlayerInput)
        {
            if (this.IsNotAFreaking<Player>())
            {
                if (this.IsNextToPlayer())
                {
                    BigBoss.Player.attack(this);
                }
            }
        }
        else if (BigBoss.PlayerInput.spellInput)
        {
            BigBoss.Gooey.Target(this);
        }
    }
    #endregion
}
