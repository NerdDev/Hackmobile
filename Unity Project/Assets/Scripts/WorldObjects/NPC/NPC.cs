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
            UpdateCurrentTileVectors();
        }
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

    //public List<Item> inventory = new List<Item>();
    public Inventory Inventory = new Inventory();
    protected List<Item> EquippedItems = new List<Item>();
    public Equipment Equipment = null;
    #endregion

    /**
     * Anything relating to movement.
     */
    #region NPC Movement Properties

    public float Speed = 1.5f;  //temporarily hard-coded
    public float NPCSpeed { get { return Speed; } }

    private float _rotationSpeed = .5f;  //temporarily hard-coded
    public float NPCRotationSpeed { get { return _rotationSpeed; } }

    public Vector3 CurrentOccupiedGridCenterWorldPoint { get; set; }
    public Vector3 LastOccupiedGridCenterWorldPoint { get; set; }

    bool moving; //stores moving condition
    protected bool verticalMoving;
    protected bool movingUp;
    protected Vector3 targetVerticalCoords;
    protected float verticalOffset;
    Vector3 targetGridCoords; //this refs target grid coords in pathing
    Vector3 heading; //this is the heading of target minus current location

    Animator animator;
    float velocity;

    #endregion

    public NPC()
    {
        Equipment = new Equipment(Bodyparts);
        Stats.MaxEncumbrance = getMaxInventoryWeight();
        Stats.Encumbrance = getCurrentInventoryWeight();
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
        Stats.Encumbrance = getCurrentInventoryWeight();
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
        if (!checkPosition(GO.transform.position, targetGridCoords))
        {
            MoveNPCStepwise(targetGridCoords);
        }
        else
        {
            moving = false;
            GO.transform.position = targetGridCoords;
            UpdateCurrentTileVectors();
        }
    }

    protected void verticalMovement()
    {
        if (!checkVerticalPosition(GO.transform.position, targetVerticalCoords))
        {
            MoveNPCStepwiseUp();
        }
        else
        {
            verticalMoving = false;
            Vector3 pos = GO.transform.position;
            pos.y = targetVerticalCoords.y;
            GO.transform.position = pos;
        }
    }

    public void move(int x, int y)
    {
        moving = true;
        targetGridCoords = new Vector3(CurrentOccupiedGridCenterWorldPoint.x - x, -.5f, CurrentOccupiedGridCenterWorldPoint.z - y);
        heading = targetGridCoords - GO.transform.position;
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
        targetVerticalCoords = new Vector3(CurrentOccupiedGridCenterWorldPoint.x, CurrentOccupiedGridCenterWorldPoint.y + z, CurrentOccupiedGridCenterWorldPoint.z);
        Vector3 pos = CurrentOccupiedGridCenterWorldPoint;
        pos.y += verticalOffset;
        CurrentOccupiedGridCenterWorldPoint = pos;
    }

    void MoveNPCStepwise(Vector3 gridCoords)
    {
        heading = gridCoords - GO.transform.position;
        GO.transform.Translate(Vector3.forward * NPCSpeed * Time.deltaTime, Space.Self);
        Quaternion toRot = Quaternion.LookRotation(heading);
        GO.transform.rotation = Quaternion.Slerp(GO.transform.rotation, toRot, NPCRotationSpeed);
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
    protected bool checkPosition(Vector3 playPos, Vector3 curPos)
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

    protected virtual bool UpdateCurrentTileVectors()
    {
        if (GridSpace != null)
        {
            GridSpace.Remove(this);
        }
        GridCoordinate = new Vector2(GO.transform.position.x.Round(), GO.transform.position.z.Round());
        GridSpace = BigBoss.Levels.Level[GridCoordinate.x.ToInt(), GridCoordinate.y.ToInt()];
        GridSpace.Put(this);
        CurrentOccupiedGridCenterWorldPoint = new Vector3(GridCoordinate.x, -.5f + verticalOffset, GridCoordinate.y);
        return true;
    }

    public PathTree getPathTree(GridSpace dest)
    {
        PathTree path = new PathTree(GridSpace, dest);
        return path;
    }
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

    public void MoveNPC(GridSpace node)
    {
        GridSpace grid = BigBoss.Levels.Level[node.X, node.Y];
        if (!grid.IsBlocked() && subtractPoints(BigBoss.Time.regularMoveCost))
        {
            int xmove = GridSpace.X - node.X;
            int ymove = GridSpace.Y - node.Y;
            GridSpace.Remove(this);
            GridSpace = BigBoss.Levels.Level[node.X, node.Y];
            GridSpace.Put(this);
            MoveNPC(xmove, ymove);
        }
    }

    public void MoveNPC(int x, int y)
    {
        moving = true;
        move(x, y);
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
        Stats.Encumbrance += item.props.Weight * count;
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
        if (this.Inventory.Remove(i))
        {
            space.Put(i);
            return true;
        }
        return false;
    }

    internal bool pickUpItem(Item i, GridSpace space)
    {
        if (space.inventory.Remove(i))
        {
            this.addToInventory(i);
            return true;
        }
        return false;
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

            //AdjustHunger(-1);
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
        if (IsNextToPlayer())
        {
            AIAttack();
        }
        else
        {
            AIMove();
        }
    }

    private void AIAttack()
    {
        if (subtractPoints(BigBoss.TimeKeeper.attackCost))
        {
            attack(BigBoss.Player);
        }
    }

    private void AIMove()
    {
        PathTree pathToPlayer = getPathTree(BigBoss.Player.GridSpace);
        if (pathToPlayer != null)
        {
            List<PathNode> nodes = pathToPlayer.getPath();
            if (nodes.Count > 2)
            {
                GridSpace nodeToMove = nodes[nodes.Count - 2].loc;
                MoveNPC(nodeToMove);
            }
            UpdateCurrentTileVectors();
        }
    }

    #endregion

    #region Touch Input
    public override void OnClick()
    {
        if (this.IsNotAFreaking<Player>())
        {
            PathTree pathToPlayer = getPathTree(BigBoss.Player.GridSpace);
            List<PathNode> nodes = pathToPlayer.getPath();
            if (nodes.Count == 2)
            {
                BigBoss.Player.attack(this);
            }
        }
    }
    #endregion
}
