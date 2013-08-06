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
        BigBoss.WorldObjectManager.AddNPCToMasterList(this);
        
        BigBoss.TimeKeeper.RegisterToUpdateList(this);
    }

    public virtual void DestroyThisItem()
    {
        BigBoss.WorldObjectManager.RemoveNPCFromMasterList(this);
        BigBoss.TimeKeeper.RemoveFromUpdateList(this);
        Destroy(this.gameObject);
    }
    #endregion

    //TODO:
    //  -- Refactor all public access to get/set functions? (annoying with unity, can't modify then).
    //  -- Refine the NPC's movement functions?

    #region Base NPC Properties
    //Local variables
    public List<NPCItem> baseInventory = null;
    #endregion

    #region NPC Instance Properties
    //Initialized all to null for the base.
    //Converted from base properties upon creation of instance.
    public Dictionary<Item, int> inventory = new Dictionary<Item,int>();
    protected List<Item> equippedItems = new List<Item>();
    protected Equipment equipment = null;
    #endregion

    #region Generic NPC Properties
    //Properties stored here.
    public GenericFlags<Properties> properties = new GenericFlags<Properties>();
    //When accessing, use the long value of NPC Properties.
    protected NPCEffect[] effects = new NPCEffect[(long)Properties.LAST];

    //All sets of flags
    GenericFlags<NPCFlags> flags = new GenericFlags<NPCFlags>();

    //Enums
    public Race race;
    public Role role;

    //Separate classes
    public AttributesData attributes = new AttributesData();
    public BodyParts bodyparts = new BodyParts();
    public Stats stats = new Stats();
    
    #endregion

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
    private Value2D<GridSpace> gridSpace;

    #endregion

    public NPC()
    {
    }

    void Start()
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

    private void killThisNPC()
    {
        //do all the calculations/etc here
        //drop the items here
        //etc etc

        if (this.IsNotAFreaking<Player>())
        {
            BigBoss.Gooey.CreateTextPop(this.gameObject.transform.position, name + " is dead!", Color.red);
            DestroyThisItem();
        }
        else
        {
            Debug.Log("Player is dead! Uhh, what do we do now?");
        }
    }

    #region Stats
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
        Debug.Log("Adjusting Health: " + amount);
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
        Debug.Log("Adjusting Health: " + amount);
        if (stats.CurrentHealth - amount > 0)
        {
            stats.CurrentHealth = stats.CurrentHealth - amount;
            BigBoss.Gooey.CreateTextPop(this.gameObject.transform.position, "Damaged for " + amount + "!", Color.red);
            return false;
        }
        else
        {
            //NPC is now dead!
            this.killThisNPC();
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

    public void MoveNPC(int x, int y)
    {
        //Debug.Log(this.gameObject.name);
        Vector3 gridCoords = new Vector3(CurrentOccupiedGridCenterWorldPoint.x - x, -.5f, CurrentOccupiedGridCenterWorldPoint.z - y);
        Vector3 heading = gridCoords - CurrentOccupiedGridCenterWorldPoint;

        //Debug.Log("Starting move sequence with: gridcoords - " + gridCoords + " and heading - " + heading);
        //while (!checkPosition(this.gameObject.transform.position, gridCoords))
        //{
        //    MoveNPCStepwise(heading, gridCoords);
            //Debug.Log("Still moving!");
        //}

        this.gameObject.transform.position = gridCoords;
    }

    private void MoveNPCStepwise(Vector3 heading, Vector3 gridCoords)
    {
        //THE INCOMING HEADING VECTOR3 DOES NOT HAVE TO BE PRENORMALIZED TO BE PASSED IN - MAKE SURE TO NORMALIZE ANY HEADING CALC'S IN THE TRANS FUNCTION
        //Translation toward a precalculated heading:
        gameObject.transform.Translate(Vector3.forward * NPCSpeed * Time.deltaTime, Space.Self);
        //Lerping rotation so we don't get jitter:
        Quaternion toRot = Quaternion.LookRotation(heading);//does this need to be normalized?
        this.gameObject.transform.rotation = Quaternion.Slerp(this.gameObject.transform.rotation, toRot, NPCRotationSpeed);
        //StartCoroutine(Wait(.1f));
    }
    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
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
    List<GameObject> lightList = new List<GameObject>();
    protected void UpdateCurrentTileVectors()
    {
        if (gridSpace != null && gridSpace.val != null)
        {
            gridSpace.val.Remove(this);
        }
        GridCoordinate = new Vector2(Mathf.Round(this.gameObject.transform.position.x), Mathf.Round(this.gameObject.transform.position.z));
        gridSpace = new Value2D<GridSpace>(Convert.ToInt32(GridCoordinate.x), Convert.ToInt32(GridCoordinate.y));
        gridSpace.val = LevelManager.Level[gridSpace.x, gridSpace.y];
        gridSpace.val.Accept(this);
        LastOccupiedGridCenterWorldPoint = CurrentOccupiedGridCenterWorldPoint;
        CurrentOccupiedGridCenterWorldPoint = new Vector3(GridCoordinate.x, -.5f, GridCoordinate.y);

        //for testing the pathing
        //buildPath();
    }

    private void buildPath(GridSpace start, GridSpace dest)
    {
        if (BigBoss.TimeKeeper.turnsPassed != 0)
        {
            foreach (GameObject go in lightList)
            {
                Destroy(go);
            }
            lightList.Clear();
            //P start = new P(conv(this.gameObject.transform.position.x), conv(this.gameObject.transform.position.z));
            //P dest = new P(conv(BigBoss.Prefabs.Orc.transform.position.x), conv(BigBoss.Prefabs.Orc.transform.position.z));
            //PathTree path = new PathTree(new Value2D<GridSpace>(start, dest);
            //List<PathNode> pathNodes = path.getPath();

            //foreach (PathNode node in pathNodes)
            //{
            //    GameObject go = Instantiate(BigBoss.Prefabs.lightMarker, new Vector3(node.loc.x, .3f, node.loc.y), Quaternion.identity) as GameObject;
            //    go.transform.Rotate(Vector3.left, -90f);
            //    lightList.Add(go);
            //}
        }
    }
    public int conv(float x)
    {
        return Convert.ToInt32(x);
    }

    public PathTree getPath(Vector2 destination) 
    {
        PathTree path = new PathTree(gridSpace, new Value2D<GridSpace>(conv(destination.x), conv(destination.y)));

        return path;
    }

    #endregion

    #region Effects

    /**
     * Just some various overloads. If not specified, it is 
     * assumed to be priority 1, infinite turns, and not an item.
     */
    public virtual void applyEffect(Properties e)
    {
        applyEffect(e, 1, false, -1);
    }

    public virtual void applyEffect(Properties e, int priority)
    {
        applyEffect(e, priority, false, -1);
    }

    public virtual void applyEffect(Properties e, bool isItem)
    {
        applyEffect(e, 1, isItem, -1);
    }

    public virtual void applyEffect(Properties e, int priority, bool isItem)
    {
        applyEffect(e, priority, isItem, -1);
    }

    /**
     * Does nothing if the NONE property is applied.
     */
    public virtual void applyEffect(Properties e, int priority, bool isItem, int turnsToProcess)
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
            BigBoss.TimeKeeper.RemoveFromUpdateList(effects[(long)e]);
            effects[(long)e] = null;
        }
    }
    #endregion

    #region Actions

    public void eatItem(Item i)
    {
        if (inventory.ContainsKey(i))
        {
            //item was just eaten, take it outta that list
            if (i.itemFlags[ItemFlags.IS_EQUIPPED]) {
                unequipItem(i);
            }
            removeFromInventory(i);

            //unless you're Jose, in which case you'll be using a mac
            //and who knows what happens when mac people eat
            //no one can finish big macs anyways
        }

        //this allows what to do on an item basis to be in the item class
        i.onEatenEvent(this);

        //removes item permanently
        DestroyObject(i.gameObject);
        DestroyObject(i);
    }

    public void useItem(Item i)
    {
        if (i.isUsable())
        {
            i.onUseEvent(this);
        }
    }

    public void attack(NPC n)
    {
        if (equipment == null)
        {
            Debug.Log("Null equipment");
        }
        if (equipment.getItems(EquipTypes.HAND) != null)
        {
            List<Item> weapons = equipment.getItems(EquipTypes.HAND);
            if (weapons.Count > 0)
            {
                foreach (Item i in weapons)
                {
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

    protected int calcHandDamage()
    {
        return (new System.Random()).Next(0, attributes.Strength);
    }
    #endregion

    #region Equipment Management

    public bool equipItem(Item i)
    {
        if (equipment.equipItem(i))
        {
            i.onEquipEvent(this);
            equippedItems.Add(i);
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
            return true;
        }
        return false;
    }

    public List<Item> getEquippedItems()
    {
        return equippedItems;
    }

    #endregion

    #region Inventory
    public void addToInventory(Item item)
    {
        this.addToInventory(item, 1);
    }

    public void addToInventory(Item item, int count)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item] += count;
        }
        else
        {
            inventory.Add(item, count);
        }
        stats.Encumbrance += item.Weight * count;
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
            stats.Encumbrance -= item.Weight * count;
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
            tempWeight += kvp.Key.Weight * kvp.Value;
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
    #endregion

    #region Get/Set of flags
    public bool get(NPCFlags fl)
    {
        return flags[fl];
    }

    public void set(NPCFlags fl, bool on)
    {
        flags[fl] = on;
    }
    #endregion

    #region NPC Management for instances
    //These are probably incomplete at this point, I haven't updated them consistently.
    public void setData(string npcName)
    {
        this.setData(BigBoss.WorldObjectManager.getNPC(npcName));
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
        IsActive = false;
    }

    #region XML Parsing
    public override void parseXML(XMLNode x)
    {
        //name is handled in DataManager so we get the GameObject name
        base.parseXML(x);

        this.role = x.SelectEnum<Role>("role");
        this.race = x.SelectEnum<Race>("race");

        //property parsing
        List<XMLNode> xprops = x.select("properties").selectList("property");
        foreach (XMLNode xnode in xprops)
        {
            Properties np = Nifty.StringToEnum<Properties>(xnode.SelectString("name"));
            NPCEffect eff = new NPCEffect(np, this);
            eff.apply(xnode.SelectInt("val"), false);
            this.effects[(long) np] = new NPCEffect(np, this);
        }

        //flag parsing
        List<XMLNode> xflags = x.select("flags").selectList("flag");
        foreach (XMLNode xnode in xflags)
        {
            NPCFlags np = Nifty.StringToEnum<NPCFlags>(xnode.SelectString("name"));
            this.flags[np] = true;
        }

        //stats
        this.stats.parseXML(x.select("stats"));

        //body part data
        this.bodyparts.parseXML(x.select("bodyparts"));

        //attributes
        this.attributes.parseXML(x.select("attributes"));

        //inventory
        baseInventory = new List<NPCItem>();
        foreach (XMLNode xnode in x.select("inventory").get())
        {
            NPCItem baseItem = new NPCItem();
            baseItem.parseXML(xnode);
            this.baseInventory.Add(baseItem);
        }
    }
    #endregion

    #endregion

    #region Turn Management

    private int npcPoints = 0;
    private int baseNPCPoints = 60;
    

    public override void UpdateTurn()
    {
        if (IsActive && BigBoss.TimeKeeper.turnsPassed != 0)
        {
            //UpdateCurrentTileVectors();
            
            try
            {
                if (this.IsNotAFreaking<Player>())
                {
                    //MoveNPC(1, 1);
                    PathTree pathToPlayer = getPath(BigBoss.PlayerInfo.gridCoordinate);
                    List<PathNode> nodes = pathToPlayer.getPath();
                    if (nodes.Count > 2)
                    {
                        move(nodes);
                        UpdateCurrentTileVectors();
                    }
                    else
                    {
                        attack(BigBoss.PlayerInfo);
                    }
                }
            }
            catch (NullReferenceException)
            {
                //do nothing
            }
            
            AdjustHunger(-1);
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

    private void move(List<PathNode> nodes)
    {
        Value2D<GridSpace> firstDest = nodes[nodes.Count - 2].loc;
        MoveNPC(gridSpace.x - firstDest.x, gridSpace.y - firstDest.y);
    }

    #endregion
}
