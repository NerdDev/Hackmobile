using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using XML;


public class NPC : Affectable
{
    /**
     * Initialization Methods
     */
    #region BIGBOSSMANAGEMENT
    // Use this for initialization
    void Awake()
    {
        if (this.IsNotAFreaking<Player>())
        {
            RegisterNPCToSingleton();
        }
        BigBoss.Time.RegisterToUpdateList(this);
    }

    public virtual void RegisterNPCToSingleton()
    {
        BigBoss.WorldObject.AddNPCToMasterList(this);
    }

    public virtual void DestroyThisItem()
    {
        BigBoss.WorldObject.RemoveNPCFromMasterList(this);
        BigBoss.Time.RemoveFromUpdateList(this);
        BigBoss.Destroy(GO);
    }

    /**
     * This method should be phased out down the line, it is used for temporary
     * data that needs initialized while other information is not known.
     */
    public void init()
    {
        calcStats();
        bodyparts.Arms = 2;
        bodyparts.Legs = 2;
        bodyparts.Heads = 1;
        equipment = new Equipment(this.bodyparts);
        if (IsActive)
        {
            UpdateCurrentTileVectors();
        }
    }
    #endregion

    /**
     * All the properties of the NPC should be contained here.
     */
    #region NPC Properties
    public ESFlags<NPCFlags> flags = new ESFlags<NPCFlags>();
    public ESFlags<Keywords> keywords = new ESFlags<Keywords>();
    public Race race;
    public Role role;
    public AttributesData attributes = new AttributesData();
    public BodyParts bodyparts = new BodyParts();
    public Stats stats = new Stats();

    //public List<Item> inventory = new List<Item>();
    public Inventory inventory = new Inventory();
    protected List<Item> equippedItems = new List<Item>();
    protected Equipment equipment = null;
    #endregion

    /**
     * Anything relating to movement.
     */
    #region NPC Movement Properties

    public float speed = 1.5f;  //temporarily hard-coded
    public float NPCSpeed { get { return speed; } }

    public float rotationSpeed = .5f;  //temporarily hard-coded
    public float NPCRotationSpeed { get { return rotationSpeed; } }

    private Vector3 currentOccupiedGridCenterWorldPoint;
    public Vector3 CurrentOccupiedGridCenterWorldPoint
    {
        get { return currentOccupiedGridCenterWorldPoint; }
        set { currentOccupiedGridCenterWorldPoint = value; }
    }

    private Vector3 lastOccupiedGridCenterWorldPoint;
    public Vector3 LastOccupiedGridCenterWorldPoint
    {
        get { return lastOccupiedGridCenterWorldPoint; }
        set { lastOccupiedGridCenterWorldPoint = value; }
    }

    //X,Y coordinate for other scripts to grab:
    private Vector2 gridCoordinate;
    public Vector2 GridCoordinate
    {
        get { return gridCoordinate; }
        set { gridCoordinate = value; }
    }
    //X, Y in integers, GridSpace ref
    protected Value2D<GridSpace> gridSpace;
    protected Value2D<GridSpace> newGridSpace;

    bool moving; //stores moving condition
    protected bool verticalMoving;
    protected bool movingUp;
    protected Vector3 targetVerticalCoords;
    protected float verticalOffset;
    Vector3 targetGridCoords; //this refs target grid coords in pathing
    Vector3 heading; //this is the heading of target minus current location

    Animator animator;
    float vel;

    #endregion

    public NPC()
    {
        this.equipment = new Equipment(this.bodyparts);
        stats.MaxEncumbrance = getMaxInventoryWeight();
        stats.Encumbrance = getCurrentInventoryWeight();
        stats.CurrentHealth = stats.MaxHealth;
        stats.CurrentPower = stats.MaxPower;
        stats.XPToNextLevel = calcXPForNextLevel();
        stats.CurrentXP = 0;
        stats.hungerRate = 1;
    }

    void Start()
    {
        animator = GO.GetComponent<Animator>() as Animator;
    }

    void Update()
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

    void FixedUpdate()
    {
        if (moving)
        {
            if (vel < NPCSpeed)
            {
                vel += .01f;
            }
            else
            {
                vel = NPCSpeed;
            }
        }
        else
        {
            vel = 0;
        }
        if (animator == null)
        {
            animator = GO.GetComponent<Animator>() as Animator;
        }
        else
        {
            animator.SetFloat("runSpeed", vel);
        }
    }

    #region Stats
    public bool get(NPCFlags fl)
    {
        return flags[fl];
    }

    public void set(NPCFlags fl, bool on)
    {
        flags[fl] = on;
    }

    public virtual float AdjustHunger(float amount)
    {
        stats.Hunger += amount;
        Debug.Log("Gained " + amount + " of nutrition.");
        getHungerLevel(stats.Hunger);
        return stats.Hunger;
    }

    public virtual void AddLevel()
    {
        stats.Level++;
        //do level up stuff here
        calcStats();
    }

    public virtual void AdjustHealth(int amount)
    {
        if (amount < 0)
        {
            damage(-amount);
        }
        else if (stats.CurrentHealth + amount > stats.MaxHealth)
        {
            stats.CurrentHealth = stats.MaxHealth;
            Debug.Log(this.Name + " gained " + amount + " in health.");
        }
        else
        {
            stats.CurrentHealth = stats.CurrentHealth + amount;
            Debug.Log(this.Name + " gained " + amount + " in health.");
        }
    }

    public virtual bool damage(int amount)
    {
        if (stats.CurrentHealth - amount > 0)
        {
            stats.CurrentHealth = stats.CurrentHealth - amount;
            //Debug.Log(this.Name + " was damaged for " + amount + "!");
            BigBoss.Gooey.CreateTextPop(GO.transform.position, "Damaged for " + amount + "!", Color.red);
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
        stats.MaxHealth += amount;
    }

    public virtual void AdjustXP(float amount)
    {
        stats.CurrentXP += amount;
        if (stats.CurrentXP > stats.XPToNextLevel)
        {
            AddLevel();
        }
    }

    public virtual void AdjustAttribute(Attributes attr, int amount)
    {
        attributes.set(attr, attributes.get(attr) + amount);
        calcStats();
    }
    #endregion

    #region Stat Calculations
    //Use this for a re-calc on level up or any attribute changes.
    protected void calcStats()
    {
        stats.Encumbrance = getCurrentInventoryWeight();
        stats.MaxEncumbrance = getMaxInventoryWeight();
        stats.XPToNextLevel = calcXPForNextLevel();
    }

    protected float calcXPForNextLevel()
    {
        //do calc here
        return (100 + ((Mathf.Pow(stats.Level, 3f) / 2)));
    }

    protected void getHungerLevel(float hunger)
    {
        HungerLevel prior = stats.HungerLevel;
        Color col = Color.white;

        if (hunger < 50)
        {
            stats.HungerLevel = HungerLevel.Faint;
            col = Color.red;
            BigBoss.Gooey.UpdateHungerText(col);
        }
        else if (hunger < 130)
        {
            stats.HungerLevel = HungerLevel.Starving;
            col = Color.yellow;
            BigBoss.Gooey.UpdateHungerText(col);
        }
        else if (hunger < 500)
        {
            stats.HungerLevel = HungerLevel.Hungry;
            col = Color.yellow;
            BigBoss.Gooey.UpdateHungerText(col);
        }
        else if (hunger < 800)
        {
            stats.HungerLevel = HungerLevel.Satiated;
            col = Color.blue;
            BigBoss.Gooey.UpdateHungerText(col);
        }
        else if (hunger < 1000)
        {
            stats.HungerLevel = HungerLevel.Stuffed;
            col = Color.yellow;
            BigBoss.Gooey.UpdateHungerText(col);
        }

        if (prior != stats.HungerLevel)
        {
            UpdateHungerLevel(col);
        }
    }

    protected void UpdateHungerLevel(Color guiCol)
    {
        BigBoss.Gooey.CreateTextPop(GO.transform.position + Vector3.up * .75f, stats.HungerLevel.ToString() + "!", guiCol);
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
        if (gridSpace != null && gridSpace.val != null)
        {
            gridSpace.val.Remove(this);
        }
        GridCoordinate = new Vector2(GO.transform.position.x.Round(), GO.transform.position.z.Round());
        gridSpace = new Value2D<GridSpace>(GridCoordinate.x.ToInt(), GridCoordinate.y.ToInt());
        Level level = BigBoss.Levels.Level;
        gridSpace.val = level[gridSpace.x, gridSpace.y];
        gridSpace.val.Put(this);
        CurrentOccupiedGridCenterWorldPoint = new Vector3(GridCoordinate.x, -.5f + verticalOffset, GridCoordinate.y);
        return true;
    }

    public PathTree getPathTree(int x, int y)
    {
        PathTree path = new PathTree(gridSpace, new Value2D<GridSpace>(x, y));
        return path;
    }
    #endregion

    #region Actions

    public virtual void eatItem(Item i)
    {
        Debug.Log("Eating item");
        //enforces it being in inventory, if that should change we'll rewrite later
        if (inventory.Has(i))
        {
            //item was just eaten, take it outta that list
            if (i.itemFlags[ItemFlags.IS_EQUIPPED])
            {
                unequipItem(i);
            }
            i.onEatenEvent(this);
            removeFromInventory(i);

            //unless you're Jose, in which case you'll be using a mac
            //and who knows what happens when mac people eat
            //no one can finish big macs anyways

            //removes item permanently
            //DestroyObject(i.gameObject);
            //DestroyObject(i);
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
        if (equipment.getItems(EquipTypes.HAND) != null)
        {
            List<Item> weapons = equipment.getItems(EquipTypes.HAND);
            if (weapons.Count > 0)
            {
                foreach (Item i in weapons)
                {
                    Debug.Log("The " + this.Name + " swings with his " + i.Name + "!");
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
                Debug.Log("The " + this.Name + " swings with his bare hands!");
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
            Debug.Log("The " + this.Name + " swings with his bare hands!");
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

    public void MoveNPC(Value2D<GridSpace> node)
    {
        GridSpace grid = BigBoss.Levels.Level[node.x, node.y];
        if (!grid.IsBlocked() && subtractPoints(BigBoss.Time.regularMoveCost))
        {
            int xmove = gridSpace.x - node.x;
            int ymove = gridSpace.y - node.y;
            gridSpace.val.Remove(this);
            gridSpace = new Value2D<GridSpace>(node.x, node.y, grid);
            gridSpace.val.Put(this);
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
        return (new System.Random()).Next(0, attributes.Strength);
    }

    private void killThisNPC()
    {
        //do all the calculations/etc here
        //drop the items here
        //etc etc

        if (this.IsNotAFreaking<Player>())
        {
            BigBoss.Gooey.CreateTextPop(GO.transform.position, Name + " is dead!", Color.red);
            Debug.Log(this.Name + " was killed!");
            DestroyThisItem();
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
        if (inventory.Has(item))
        {
            //do nothing
        }
        else
        {
            for (int i = 0; i < count - 1; i++)
            {
                inventory.Add(item);
                inventory.Add(item.Copy());
            }
        }
        stats.Encumbrance += item.props.Weight * count;
        Debug.Log("Item " + item.Name + " with count " + count + " added to inventory.");
    }

    public void removeFromInventory(Item item)
    {
        removeFromInventory(item, 1);

    }

    public void removeFromInventory(Item item, int count)
    {
        if (inventory.Has(item))
        {
            inventory.Remove(item);
            //if (inventory[item] <= count)
            //{
            //    inventory.Remove(item);
            //}
            //else
            //{
            //    inventory[item] -= count;
            //}
            stats.Encumbrance -= item.props.Weight;
            Debug.Log("Item " + item.Name + " with count " + count + " removed from inventory.");
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
        invWeightMax = (25 * (attributes.Strength + attributes.Constitution) + 50);
        return invWeightMax;
    }

    public bool equipItem(Item i)
    {
        if (equipment.equipItem(i))
        {
            i.onEquipEvent(this);
            equippedItems.Add(i);
            Debug.Log("Item " + i.Name + " equipped.");
            return true;
        }
        return false;
    }

    public bool unequipItem(Item i)
    {
        if (i.isUnEquippable() && equipment.removeItem(i))
        {
            i.onUnEquipEvent(this);
            if (equippedItems.Contains(i))
            {
                equippedItems.Remove(i);
            }
            Debug.Log("Item " + i.Name + " uneqipped.");
            return true;
        }
        return false;
    }

    public List<Item> getEquippedItems()
    {
        return equippedItems;
    }
    #endregion

    #region NPC Data Management for Instances
    public override void ParseXML(XMLNode x)
    {
        base.ParseXML(x);
        race = x.SelectEnum<Race>("race");
        role = x.SelectEnum<Role>("role");
        attributes = x.Select<AttributesData>("attributes");
        bodyparts = x.Select<BodyParts>("bodyparts");
        stats = x.Select<Stats>("stats");
        flags = x.Select<ESFlags<NPCFlags>>("flags");
        keywords = x.Select<ESFlags<Keywords>>("keywords");
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
        Surrounding<GridSpace> s = BigBoss.Levels.Level.Surrounding;
        s.Load(BigBoss.Player.gridSpace.x, BigBoss.Player.gridSpace.y);
        foreach (Value2D<GridSpace> grid in s)
        {
            if (grid.x == gridSpace.x && grid.y == gridSpace.y)
            {
                //NPC is next to player
                return true;
            }
        }
        return false;
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
            attack(BigBoss.PlayerInfo);
        }
    }

    private void AIMove()
    {
        PathTree pathToPlayer = getPathTree(BigBoss.Player.gridSpace.x, BigBoss.Player.gridSpace.y);
        if (pathToPlayer != null)
        {
            List<PathNode> nodes = pathToPlayer.getPath();
            Value2D<GridSpace> nodeToMove = nodes[nodes.Count - 2].loc;
            MoveNPC(nodeToMove);
            UpdateCurrentTileVectors();
        }
    }

    #endregion

    #region Touch Input
    void OnEnable()
    {
        EasyTouch.On_SimpleTap += On_SimpleTap;
    }

    void OnDisable()
    {
        EasyTouch.On_SimpleTap -= On_SimpleTap;
    }

    void On_SimpleTap(Gesture gesture)
    {
        if (gesture.pickObject != null)
        {
            GameObject go = gesture.pickObject;
            if (GO == go)
            {
                NPC n = go.GetComponent<WOInstance<NPC>>();
                if (this.IsNotAFreaking<Player>() && this == n)
                {
                    PathTree pathToPlayer = getPathTree(BigBoss.Player.gridSpace.x, BigBoss.Player.gridSpace.y);
                    List<PathNode> nodes = pathToPlayer.getPath();
                    if (nodes.Count == 2)
                    {
                        BigBoss.Player.attack(this);
                    }
                }
            }
        }
    }
    #endregion
}
