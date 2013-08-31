using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using XML;
/**
 * Notes on usage:
 * 
 * Adding a property to an NPC requires adding it in several places;
 *  - The property itself
 *  - In the XML definition (if applicable)
 *  - In the parseXML (if read from XML)
 *  - In the setNull definition (to define a a null NPC)
 *  - In the setData definition (to define an instance of an NPC)
 *  
 * When adding a property, if the property is only useful or applicable to a base NPC
 *  then store it under the region of base NPC properties, and set it to a null initialization.
 *  Add the definition to convert it to whatever is needed for the instance of an NPC
 *  under the setData method.
 *  
 * If it is only applicable to an instance of an NPC, then do the opposite - add it to
 *  the instance NPC properties, and leave the initialization as null until it is defined
 *  in the setData method.
 *  
 * See equipment and equipped items for an example.
 */
public class NPC : WorldObject
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
    }

    public virtual void RegisterNPCToSingleton()
    {
        BigBoss.WorldObject.AddNPCToMasterList(this);
        BigBoss.Time.RegisterToUpdateList(this);
    }

    public virtual void DestroyThisItem()
    {
        BigBoss.WorldObject.RemoveNPCFromMasterList(this);
        BigBoss.Time.RemoveFromUpdateList(this);
        Destroy(this.gameObject);
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

    public GenericFlags<Properties> properties = new GenericFlags<Properties>();
    //When accessing, use the long value of NPC Properties.
    protected NPCEffect[] effects = new NPCEffect[(long)Properties.LAST];
    GenericFlags<NPCFlags> flags = new GenericFlags<NPCFlags>();
    public Race race;
    public Role role;
    public AttributesData attributes = new AttributesData();
    public BodyParts bodyparts = new BodyParts();
    public Stats stats = new Stats();

    public Dictionary<Item, int> inventory = new Dictionary<Item, int>();
    protected List<Item> equippedItems = new List<Item>();
    protected Equipment equipment = null;

    protected bool player = false;
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
    Vector3 targetGridCoords; //this refs target grid coords in pathing
    Vector3 heading; //this is the heading of target minus current location

    #endregion

    public NPC()
    {
    }

    void Start()
    {
    }

    void Update()
    {
        if (IsActive)
        {
            if (moving)
            {
                if (!checkPosition(this.gameObject.transform.position, targetGridCoords))
                {
                    MoveNPCStepwise(targetGridCoords);
                }
                else
                {
                    moving = false;
                    this.gameObject.transform.position = targetGridCoords;
                    UpdateCurrentTileVectors();
                }
            }
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
        }
        else
        {
            stats.CurrentHealth = stats.CurrentHealth + amount;
        }
    }

    public virtual bool damage(int amount)
    {
        if (stats.CurrentHealth - amount > 0)
        {
            stats.CurrentHealth = stats.CurrentHealth - amount;
            //Debug.Log(this.Name + " was damaged for " + amount + "!");
            BigBoss.Gooey.CreateTextPop(this.gameObject.transform.position, "Damaged for " + amount + "!", Color.red);
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
        BigBoss.Gooey.CreateTextPop(this.gameObject.transform.position + Vector3.up * .75f, stats.HungerLevel.ToString() + "!", guiCol);
    }

    public float getXPfromNPC()
    {
        //how much XP is this NPC worth?
        return 10f;
    }
    #endregion

    #region Movement
    public void move(int x, int y)
    {
        moving = true;
        targetGridCoords = new Vector3(CurrentOccupiedGridCenterWorldPoint.x - x, -.5f, CurrentOccupiedGridCenterWorldPoint.z - y);
        heading = targetGridCoords - this.gameObject.transform.position;
    }

    private void MoveNPCStepwise(Vector3 gridCoords)
    {
        heading = gridCoords - this.gameObject.transform.position;
        gameObject.transform.Translate(Vector3.forward * NPCSpeed * Time.deltaTime, Space.Self);
        Quaternion toRot = Quaternion.LookRotation(heading);
        this.gameObject.transform.rotation = Quaternion.Slerp(this.gameObject.transform.rotation, toRot, NPCRotationSpeed);
    }

    private float variance = .08f;
    private bool checkPosition(Vector3 playPos, Vector3 curPos)
    {
        if (Math.Abs(playPos.x - curPos.x) > variance ||
            Math.Abs(playPos.z - curPos.z) > variance)
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
        GridCoordinate = new Vector2(this.gameObject.transform.position.x.Round(), this.gameObject.transform.position.z.Round());
        gridSpace = new Value2D<GridSpace>(GridCoordinate.x.ToInt(), GridCoordinate.y.ToInt());
        gridSpace.val = LevelManager.Level[gridSpace.x, gridSpace.y];
        gridSpace.val.Put(this);
        CurrentOccupiedGridCenterWorldPoint = new Vector3(GridCoordinate.x, -.5f, GridCoordinate.y);
        return true;
    }

    public PathTree getPathTree(int x, int y)
    {
        PathTree path = new PathTree(gridSpace, new Value2D<GridSpace>(x, y));
        return path;
    }

    #endregion

    #region Effects
    /**
     * Does nothing if the NONE property is applied.
     */
    public virtual void applyEffect(Properties e, int priority = 1, bool isItem = false, int turnsToProcess = -1)
    {
        if (e != Properties.NONE)
        {
            if (effects[(long)e] == null)
            {
                NPCEffect eff = new NPCEffect(e, this);
                if (this.properties[Properties.NONE])
                {
                    this.properties[Properties.NONE] = false;
                }

                effects[(long)e] = eff;
                eff.apply(priority, isItem, turnsToProcess);

            }
            else
            {
                effects[(long)e].apply(priority, isItem, turnsToProcess);
            }
        }
        BigBoss.Gooey.CreateTextPop(this.gameObject.transform.position, e.ToString(), Color.green);
    }

    /**
     * This entirely removes the effect from the NPC. If applied again later, it'll create a new one.
     */
    public void removeEffect(Properties e)
    {
        if (effects[(long)e] != null)
        {
            BigBoss.Time.RemoveFromUpdateList(effects[(long)e]);
            effects[(long)e] = null;
        }
    }
    #endregion

    #region Actions

    public void eatItem(Item i)
    {
        //enforces it being in inventory, if that should change we'll rewrite later
        if (inventory.ContainsKey(i))
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

            //this allows what to do on an item basis to be in the item class
            i.onEatenEvent(this);

            //removes item permanently
            //DestroyObject(i.gameObject);
            //DestroyObject(i);
        }
    }

    public void useItem(Item i)
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
        GridSpace grid = LevelManager.Level[node.x, node.y];
        if (!grid.IsBlocked())
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
            BigBoss.Gooey.CreateTextPop(this.gameObject.transform.position, name + " is dead!", Color.red);
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
        if (inventory.ContainsKey(item))
        {
            inventory[item] += count;
        }
        else
        {
            inventory.Add(item, count);
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
        if (inventory.ContainsKey(item))
        {
            if (inventory[item] <= count)
            {
                inventory.Remove(item);
            }
            else
            {
                inventory[item] -= count;
            }
            stats.Encumbrance -= item.props.Weight * count;
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
        foreach (KeyValuePair<Item, int> kvp in inventory)
        {
            tempWeight += kvp.Key.props.Weight * kvp.Value;
        }

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
    public void setData(string npcName)
    {
        this.setData(BigBoss.WorldObject.getNPC(npcName));
    }

    public void setData(NPC npc)
    {
        //Anything performing the conversion from base NPC -> instance of NPC goes here.
        base.setData(npc);
        //classes
        this.stats = npc.stats.Copy();
        this.attributes = npc.attributes.Copy();
        this.bodyparts = npc.bodyparts.Copy();
        //flags
        this.flags = npc.flags.Copy();
        //enums
        this.race = npc.race;
        this.role = npc.role;
        //lists
        this.effects = npc.effects.Copy();
        //inventory
        inventory = new Dictionary<Item, int>();
        //foreach (NPCItem nitem in baseInventory)
        //{
        //    //go to inventory
        //}
        equippedItems = new List<Item>();
        equipment = new Equipment(this.bodyparts);
        stats.MaxEncumbrance = getMaxInventoryWeight();
        stats.Encumbrance = getCurrentInventoryWeight();
        stats.CurrentHealth = stats.MaxHealth;
        stats.CurrentPower = stats.MaxPower;
        stats.XPToNextLevel = calcXPForNextLevel();
        stats.CurrentXP = 0;
        stats.hungerRate = 1;
    }

    public override void setNull()
    {
        this.parseXML(new XMLNode(null));
        /*
        base.setNull();
        this.attributes.setNull();
        this.bodyparts.setNull();
        if (equipment != null)
        {
            this.equipment.setNull();
        }  
        this.flags.setNull();
        this.race = Race.NONE;
        this.role = Role.NONE;
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i] = null;
        }
        */
        IsActive = false;
    }

    public override void parseXML(XMLNode x)
    {
        //name is handled in DataManager so we get the GameObject name
        base.parseXML(x);

        //Use XMLNifty as it will handle any null refs
        this.role = XMLNifty.SelectEnum<Role>(x, "role");
        this.race = XMLNifty.SelectEnum<Race>(x, "race");

        #region Specialized parsing
        //property parsing
        XMLNifty.parseList(x, "properties", "property",
            obj =>
            {
                Properties np = obj.SelectEnum<Properties>("name");
                NPCEffect eff = new NPCEffect(np, this);
                eff.apply(obj.SelectInt("val"), false);
                this.effects[(long)np] = new NPCEffect(np, this);
            });

        //flag parsing
        XMLNifty.parseList(x, "flags", "flag",
            obj =>
            {
                NPCFlags np = obj.SelectEnum<NPCFlags>("name");
                this.flags[np] = true;
            });
        #endregion

        //stats
        this.stats.parseXML(x);

        //body part data
        this.bodyparts.parseXML(x);

        //attributes
        this.attributes.parseXML(x);
    }

    #endregion

    #region Turn Management

    private int npcPoints = 0;
    private int baseNPCPoints = 60;

    public override void UpdateTurn()
    {
        if (IsActive && BigBoss.Time.turnsPassed != 0)
        {
            try
            {
                if (this.IsNotAFreaking<Player>())
                {
                    if (IsNextToPlayer())
                    {
                        NextToPlayer();
                    }
                    else
                    {
                        DecideWhatToDo();
                    }
                }
            }
            catch (NullReferenceException)
            {
                //do nothing
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
        Surrounding<GridSpace> s = LevelManager.Level.surr;
        s.Load(BigBoss.PlayerInfo.gridSpace.x, BigBoss.PlayerInfo.gridSpace.y);
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

    void NextToPlayer()
    {
        //attack?
        attack(BigBoss.PlayerInfo);
    }

    void DecideWhatToDo()
    {
        //move?
        PathTree pathToPlayer = getPathTree(BigBoss.PlayerInfo.gridSpace.x, BigBoss.PlayerInfo.gridSpace.y);
        if (pathToPlayer != null)
        {
            List<PathNode> nodes = pathToPlayer.getPath();
            MoveNPC(nodes[nodes.Count - 2].loc);
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
            if (this.gameObject == go)
            {
                NPC n = go.GetComponent<NPC>();
                if (this.IsNotAFreaking<Player>() && this == n)
                {
                    PathTree pathToPlayer = getPathTree(BigBoss.PlayerInfo.gridSpace.x, BigBoss.PlayerInfo.gridSpace.y);
                    List<PathNode> nodes = pathToPlayer.getPath();
                    if (nodes.Count == 2)
                    {
                        BigBoss.PlayerInfo.attack(this);
                    }
                }
            }
        }
    }
    #endregion
}
