using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GUIManager : MonoBehaviour, IManager
{

    /*
     * The role of GUIManager is to centralize the use of methods that can be overloaded in order
    to create reusable, yet easily customizable screen graphics.  These should all return the GUIText
    object and allow for parameterization of text, fly-through style, delay til destruction, etc.
	
    //Example of creating text on screen:
        Managers.Gooey.CreateText(GUIManager.TextLocations.TopLeft,"Works!");
    */

    #region GUI Variables
    public bool Initialized { get; set; }
    private Queue<TextPop> textPopList = new Queue<TextPop>();

    internal ItemChest currentChest;
    internal string currentSpell;
    internal GUIButton selectedButton;

    internal bool categoryDisplay = false;
    internal bool displayItem = false;
    internal string category = "";
    public bool displayInventory;
    public bool displaySpells;
    internal bool DisplayChatGrid = false;

    public Material NormalShaderGridspace;
    public Material GlowShaderGridSpace;

    internal HashSet<IAffectable> spellTargets = new HashSet<IAffectable>();
    #endregion

    #region Publicly populated variables from scene
    public ScrollingGrid InventoryGrid;
    public ScrollingGrid ItemActionsGrid;
    public ScrollingGrid GroundItemsGrid;
    public ScrollingGrid ItemInfoGrid;
    public ScrollingGrid SpellCastGrid;
    public TextGrid ChatGrid;
    public GUIProgressBar HealthBar;
    public GUIProgressBar ManaBar;

    //Prefabs
    public GameObject InvItemPrefab;
    public GameObject ButtonPrefab;
    public GameObject ChestPrefab;
    public GameObject LabelPrefab;

    public UIFont font;

    //Misc
    public GameObject InventoryLabel;
    public GameObject GroundLabel;
    public GameObject debugText;
    public GameObject textPopPrefab;
    public Light DiscoLight;
    public Texture cookie;
    public GUITexture LoadImage;

    public Shader InvisibilityShader;
    public Shader TransparentShader;
    public GameObject InvisibleObject;

    //Cameras
    public Camera GUICam;
    public Camera HeroCam;

    public FOWSystem fow;
    #endregion

    void Start()
    {
        StartCoroutine(Display());
    }

    void Update()
    {
        if (Time.time % 20 == 0)
        {
            GC.Collect();
        }
    }

    public void Initialize()
    {
    }

    public void OpenMenuGUI()
    {
        List<GameObject> buttons = new List<GameObject>();
        GUIButton startButton = CreateButton("Start");
        buttons.Add(startButton.gameObject);
        startButton.Fix();
        startButton.transform.localScale = new Vector3(.01f, .01f, .01f);
        startButton.transform.localPosition = new Vector3(0f, .5f, 0f);
        startButton.OnSingleClick = new Action(() =>
        {
            StartCoroutine(BigBoss.Starter.StartGame());
            foreach (GameObject go in buttons)
            {
                Destroy(go);
            }
        });

        GUIButton startButton2 = CreateButton("Disco Start");
        buttons.Add(startButton2.gameObject);
        startButton2.Fix();
        startButton2.transform.localScale = new Vector3(.01f, .01f, .01f);
        startButton2.transform.localPosition = new Vector3(0f, -.5f, 0f);
        startButton2.OnSingleClick = new Action(() =>
        {
            DiscoLight.color = Color.white;
            DiscoLight.cookie = cookie;
            StartCoroutine(BigBoss.Starter.StartGame());
            foreach (GameObject go in buttons)
            {
                Destroy(go);
            }
        });
    }

    public void OpenSpellGUI()
    {
        RegenSpellGUI(SpellCastGrid);
    }

    public void OpenInventoryGUI()
    {
        RegenInventoryGUI(InventoryGrid);
    }

    public void OpenGroundGUI(ItemChest chest)
    {
        displayInventory = true;
        GenerateGroundItems(chest, GroundItemsGrid);
    }

    public void DisplayLoading()
    {
        LoadImage.enabled = true;
    }

    public void CloseLoading()
    {
        LoadImage.enabled = false;
    }

    public void RecreateFOW(Vector3 pos, int height)
    {
        fow.ModifyGrid(pos, height);
    }

    /*
    public void OpenInventoryUI()
    {
        //Calling tween pos script on panel object
        invPanelTweenPos.duration = .75f;
        invPanelTweenPos.from = new Vector3(inventoryPanel.transform.localPosition.x, inventoryPanel.transform.localPosition.y, 0);//wrap these up into variables
        invPanelTweenPos.to = new Vector3(160f, inventoryPanel.transform.localPosition.y, 0);//wrap these up into variables
        invPanelTweenPos.Reset();
        invPanelTweenPos.Play(true);
        isInventoryOpen = true;
    }

    public void CloseInventoryUI()
    {
        //Calling tween pos script on panel object
        invPanelTweenPos.duration = .35f;
        invPanelTweenPos.from = new Vector3(inventoryPanel.transform.localPosition.x, inventoryPanel.transform.localPosition.y, 0);//wrap these up into variables
        invPanelTweenPos.to = new Vector3(600, inventoryPanel.transform.localPosition.y, 0);//wrap these up into variables
        invPanelTweenPos.Reset();
        invPanelTweenPos.Play(true);
        isInventoryOpen = false;
    }
    */

    #region Misc Graphical

    public GameObject SpawnShadowCaster(Vector3 pos, Vector3 scale)
    {
        GameObject go = GameObject.Instantiate(InvisibleObject, pos, Quaternion.identity) as GameObject;
        go.transform.localScale = scale;
        return go;
    }

    #endregion

    #region GUI
    public void UpdateHealthBar(int val)
    {
        HealthBar.UpdateValue(val);
    }

    public void UpdateMaxHealth(int val)
    {
        HealthBar.SetMaxValue(val);
    }

    public void UpdatePowerBar(int val)
    {
        ManaBar.UpdateValue(val);
    }

    public void UpdateMaxPower(int val)
    {
        ManaBar.SetMaxValue(val);
    }

    public void UpdateHungerText(Color theCol)
    {
        //		hungerLabel.text = BigBoss.PlayerInfo.stats.HungerLevel.ToString();
        //		hungerLabel.color = theCol;
    }

    public void UpdateXPBar()
    {
        //		HUDxpbar.sliderValue = (float)BigBoss.PlayerInfo.stats.CurrentXP/1000f;
        //		HUD2XPLabel.text = BigBoss.PlayerInfo.stats.CurrentXP + " / 1000";
    }

    public void UpdateTilesCrossedLabel()
    {
        //tilesCrossedLabel.text = "Tiles Crossed: " + BigBoss.TimeKeeper.numTilesCrossed;
    }

    public void UpdateHungerLevel(HungerLevel hunger)
    {
        Color color = Color.white;
        // These colors don't make sense with the states
        switch (hunger)
        {
            case HungerLevel.Faint:
                color = Color.red;
                break;
            case HungerLevel.Starving:
                color = Color.yellow;
                break;
            case HungerLevel.Hungry:
                color = Color.yellow;
                break;
            case HungerLevel.Satiated:
                color = Color.blue;
                break;
            case HungerLevel.Stuffed:
                color = Color.yellow;
                break;
        }
        BigBoss.Gooey.UpdateHungerText(color);
        BigBoss.Gooey.CreateTextMessage(hunger.ToString() + "!", color);
    }

    public void CreateTextPop(Vector3 worldPosition, string message) //no color arg version
    {
        CreateTextPop(worldPosition, message, Color.white);
    }

    public void CreateTextMessage(string message, Color col)
    {
        GameObject button = Instantiate(LabelPrefab) as GameObject;
        GUILabel label = button.GetComponent<GUILabel>();
        label.Text = message;
        label.UIDragPanel.draggablePanel = ChatGrid.DragPanel;
        ChatGrid.AddLabel(label);
        try
        {
            ChatGrid.Reposition();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.Source);
            Debug.Log(e.StackTrace);
        }
    }

    public void CreateTextPop(Vector3 worldPosition, string message, Color col)
    {
        TextPop text = new TextPop(message, worldPosition, col);
        textPopList.Enqueue(text);
    }

    void DisplayTextPops()
    {
        if (textPopList.Count > 80)
        {
            textPopList.Clear();
        }
        else if (textPopList.Count > 20)
        {
            //skip entries when the count gets too high
            for (int i = 0; i < 15; i++)
            {
                textPopList.Dequeue();
            }
        }
        else
        {
            textPopList.Dequeue().Display();
        }
    }

    private WaitForSeconds wfs = new WaitForSeconds(.25f);
    IEnumerator Display()
    {
        while (enabled)
        {
            if (textPopList.Count > 0)
            {
                DisplayTextPops();
            }
            yield return wfs;
        }
    }

    internal void RegenSpellGUI(ScrollingGrid grid)
    {
        if (displaySpells)
        {
            grid.gameObject.SetActive(true);
            grid.Clear();
            foreach (string key in BigBoss.Player.KnownSpells.Keys)
            {
                GUIButton spellButton = CreateObjectButton(key, grid, key);
                spellButton.OnSingleClick = new Action(() =>
                {
                    currentSpell = spellButton.refObject as string;
                    if (BigBoss.Player.Stats.CurrentPower > GetCurrentSpellCost())
                    {
                        BigBoss.PlayerInput.InputSetting[InputSettings.DEFAULT_INPUT] = false;
                        BigBoss.PlayerInput.InputSetting[InputSettings.SPELL_INPUT] = true;
                        spellButton.defaultColor = spellButton.hover;
                        selectedButton = spellButton;
                        spellButton.UpdateColor(true, true);
                    }
                    else
                    {
                        CreateTextPop(BigBoss.PlayerInfo.transform.position, "You do not have enough power to cast " + currentSpell + "!");
                        currentSpell = null;
                    }
                });
            }
            GUIButton cancelSpellButton =  CreateButton(grid, "Cancel Spell");
            cancelSpellButton.OnSingleClick = new Action(() =>
                {
                    BigBoss.PlayerInput.InputSetting[InputSettings.DEFAULT_INPUT] = true;
                    BigBoss.PlayerInput.InputSetting[InputSettings.SPELL_INPUT] = false;
                    currentSpell = null;
                    selectedButton.defaultColor = cancelSpellButton.defaultColor;
                    selectedButton.UpdateColor(true, true);

                });
            GUIButton castSpellButton = CreateButton(grid, "Cast Spell");
            castSpellButton.OnSingleClick = new Action(() =>
            {
                BigBoss.Player.CastSpell(currentSpell, spellTargets.ToArray());
                BigBoss.PlayerInput.InputSetting[InputSettings.DEFAULT_INPUT] = true;
                BigBoss.PlayerInput.InputSetting[InputSettings.SPELL_INPUT] = false;
                foreach (IAffectable target in spellTargets)
                {
                    if (target is NPC)
                    {
                        foreach (GameObject block in ((NPC)target).GridSpace.Blocks)
                        {
                            block.renderer.sharedMaterial = NormalShaderGridspace;
                        }
                    }
                }
                spellTargets.Clear();
                currentSpell = null;
				selectedButton.defaultColor = cancelSpellButton.defaultColor;
				selectedButton.UpdateColor(true, true);
            });
            grid.Reposition();
        }
        else
        {
            grid.Clear();
        }
    }

    public void Target(IAffectable target)
    {
        if (spellTargets.Contains(target))
        {
            RemoveTarget(target);
        }
        else
        {
            AddTarget(target);
        }
    }

    internal void RemoveTarget(IAffectable target)
    {
        spellTargets.Remove(target);
        if (target is NPC)
        {
            foreach (GameObject block in ((NPC)target).GridSpace.Blocks)
            {
                block.renderer.sharedMaterial = NormalShaderGridspace;
            }
        }
    }

    internal void AddTarget(IAffectable target)
    {
        spellTargets.Add(target);
        if (target is NPC)
        {
            foreach (GameObject block in ((NPC)target).GridSpace.Blocks)
            {
                block.renderer.sharedMaterial = GlowShaderGridSpace;
            }
        }
    }

    public int GetCurrentSpellRange()
    {
        if (BigBoss.Player.KnownSpells.ContainsKey(currentSpell))
        {
            return BigBoss.Player.KnownSpells[currentSpell].range;
        }
        return 0;
    }

    public int GetCurrentSpellCost()
    {
        if (BigBoss.Player.KnownSpells.ContainsKey(currentSpell))
        {
            return BigBoss.Player.KnownSpells[currentSpell].cost;
        }
        return 0;
    }

    internal void RegenInventoryGUI(ScrollingGrid grid)
    {
        if (displayInventory)
        {
            InventoryLabel.SetActive(true);
            grid.gameObject.SetActive(true);
            grid.Clear();
            if (!categoryDisplay)
            {
                foreach (InventoryCategory ic in BigBoss.Player.Inventory.Values)
                {
                    CreateCategoryButton(ic, grid);
                }
            }
            else
            {
                InventoryCategory ic;
                if (BigBoss.Player.Inventory.TryGetValue(category, out ic))
                {
                    foreach (Item item in ic.Values)
                    {
                        CreateItemButton(item, grid);
                    }
                }
                CreateBackLabel(grid);
            }
            grid.Reposition();
        }
        else
        {
            InventoryLabel.SetActive(false);
            grid.Clear();
            RegenItemInfoGUI(null);
        }
    }

    internal void GenerateGroundItems(ItemChest chest, ScrollingGrid grid)
    {
        if (displayInventory && chest != null)
        {
            currentChest = chest;
            Inventory inv = chest.Location.inventory;
            GroundLabel.SetActive(true);
            grid.gameObject.SetActive(true);
            grid.Clear();
            foreach (InventoryCategory ic in inv.Values)
            {
                foreach (Item item in ic.Values)
                {
                    CreateItemButton(item, grid);
                }
            }
            CreateCloseLabel(grid);
            grid.ResetPosition();
            grid.Reposition();
        }
        else
        {
            GroundLabel.SetActive(false);
            grid.Clear();
            RegenItemInfoGUI(null);
        }
    }

    private void CreateCloseLabel(ScrollingGrid grid)
    {
        GUIButton button = CreateButton(grid, "CloseButton", "Close");
        button.OnSingleClick = new Action(() =>
        {
            BigBoss.Gooey.category = "";
            BigBoss.Gooey.categoryDisplay = false;
            BigBoss.Gooey.OpenGroundGUI(null);
            BigBoss.Gooey.displayItem = false;
            BigBoss.Gooey.RegenItemInfoGUI(null);
        });
    }

    internal void RegenItemInfoGUI(Item item)
    {
        if (displayInventory)
        {
			ItemInfoGrid.gameObject.SetActive(true);
			ItemActionsGrid.gameObject.SetActive(true);
            ItemInfoGrid.Clear();
            ItemActionsGrid.Clear();
            if (displayItem && item != null && item.Count > 0)
            {
                GenerateItemInfo(item, ItemInfoGrid);
                GenerateItemActions(item, ItemActionsGrid);
            }
            else
            {
                OpenInventoryGUI();
                ItemInfoGrid.Clear();
                ItemActionsGrid.Clear();
				ItemInfoGrid.gameObject.SetActive(false);
				ItemActionsGrid.gameObject.SetActive(false);
            }
        }
    }

    GUIButton CreateButton(string buttonName = "Button", string buttonText = null)
    {
        GameObject button = Instantiate(ButtonPrefab) as GameObject;
        GUIButton butt = button.GetComponent<GUIButton>();
        button.name = buttonName;
        if (buttonText == null) buttonText = buttonName;
        butt.Text = buttonText;
        return butt;
    }

    GUIButton CreateButton(ScrollingGrid grid, string buttonName = "Button", string buttonText = null)
    {
        if (buttonText == null) buttonText = buttonName;
        GUIButton button = CreateButton(buttonName, buttonText);
        button.UIDragPanel.draggablePanel = grid.DragPanel;
        grid.AddButton(button);
        return button.GetComponent<GUIButton>();
    }

    GUIButton CreateObjectButton(System.Object o, ScrollingGrid grid, string buttonName = "Button", string buttonText = null)
    {
        GUIButton button = CreateButton(grid, buttonName, buttonText);
        button.refObject = o;
        return button;
    }

    void CreateBackLabel(ScrollingGrid grid)
    {
        GUIButton button = CreateButton(grid, "BackButton", "Back");
        button.OnSingleClick = new Action(() =>
        {
            BigBoss.Gooey.category = "";
            BigBoss.Gooey.categoryDisplay = false;
            BigBoss.Gooey.OpenInventoryGUI();
            BigBoss.Gooey.displayItem = false;
            BigBoss.Gooey.RegenItemInfoGUI(null);
        });
    }

    void CreateItemButton(Item item, ScrollingGrid grid)
    {
        if (grid.Grid == GroundItemsGrid.Grid) { item.OnGround = true; }
        else if (grid.Grid == InventoryGrid.Grid) { item.OnGround = false; }
        string buttonText;
        if (item.Count > 1)
        {
            buttonText = item.Name + " (" + item.Count + ")";
        }
        else { buttonText = item.Name; }
        GUIButton itemButton = CreateObjectButton(item, grid, item.Name, buttonText);
        itemButton.OnSingleClick = new Action(() =>
        {
            if ((itemButton.refObject as Item).Count > 0)
            {
                BigBoss.Gooey.displayItem = true;
                BigBoss.Gooey.RegenItemInfoGUI(itemButton.refObject as Item);
            }
        });
    }

    void CreateCategoryButton(InventoryCategory ic, ScrollingGrid grid)
    {
        GUIButton categoryButton = CreateObjectButton(ic, grid, ic.id);
        categoryButton.OnSingleClick = new Action(() =>
        {
            BigBoss.Gooey.categoryDisplay = true;
            BigBoss.Gooey.category = (categoryButton.refObject as InventoryCategory).id;
            BigBoss.Gooey.OpenInventoryGUI();
        });
    }

    void GenerateItemInfo(Item item, ScrollingGrid grid)
    {
        if (item.Count > 0)
        {
            foreach (KeyValuePair<string, string> kvp in item.GetGUIDisplays())
            {
                //display the info on the item
            }
            CreateBackLabel(grid);
            grid.ResetPosition();
            grid.Reposition();
        }
    }

    void GenerateItemActions(Item item, ScrollingGrid grid)
    {
        if (!item.itemFlags[ItemFlags.IS_EQUIPPED])
        {
            CreateEquipButton(item, grid);
        }
        else
        {
            CreateUnEquipButton(item, grid);
        }
        CreateUseButton(item, grid);
        CreateEatButton(item, grid);
        if (item.OnGround)
        {
            CreatePickUpButton(item, grid);
        }
        else
        {
            CreateDropButton(item, grid);
        }
        grid.ResetPosition();
        //itemActionClip.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        //itemActionClip.clipRange = new Vector4(500, -400, 200, 800);
        grid.Reposition();
    }

    void CreateEquipButton(Item item, ScrollingGrid grid)
    {
        GUIButton itemButton = CreateObjectButton(item, grid, "Equip Item");
        itemButton.OnSingleClick = new Action(() =>
        {
            Item i = itemButton.refObject as Item;
            if (i.OnGround == true)
            {
                BigBoss.Player.Inventory.Add(i);
                BigBoss.Player.GridSpace.Remove(i);
                BigBoss.Gooey.OpenGroundGUI(currentChest);
            }
            BigBoss.Player.equipItem(i);
            BigBoss.Gooey.RegenItemInfoGUI(i);
        });
    }

    void CreateUnEquipButton(Item item, ScrollingGrid grid)
    {
        GUIButton itemButton = CreateObjectButton(item, grid, "UnEquip Item");
        itemButton.OnSingleClick = new Action(() =>
        {
            Item i = itemButton.refObject as Item;
            if (i != null)
            {
                BigBoss.Player.unequipItem(i);
                BigBoss.Gooey.RegenItemInfoGUI(i);
            }
        });
    }

    void CreateUseButton(Item item, ScrollingGrid grid)
    {
        GUIButton itemButton = CreateObjectButton(item, grid, "Use Item");
        itemButton.OnSingleClick = new Action(() =>
        {
            Item i = itemButton.refObject as Item;
            BigBoss.Player.useItem(i);
            BigBoss.Gooey.RegenItemInfoGUI(i);
            BigBoss.Gooey.OpenInventoryGUI();
        });
    }

    void CreateDropButton(Item item, ScrollingGrid grid)
    {
        GUIButton itemButton = CreateObjectButton(item, grid, "Drop Item");
        itemButton.OnSingleClick = new Action(() =>
        {
            Item i = itemButton.refObject as Item;
            BigBoss.Player.dropItem(i, BigBoss.Player.GridSpace);
            BigBoss.Gooey.OpenInventoryGUI();
            BigBoss.Gooey.OpenGroundGUI(currentChest);
        });
    }

    void CreatePickUpButton(Item item, ScrollingGrid grid)
    {
        GUIButton itemButton = CreateObjectButton(item, grid, "Pick Up Item");
        itemButton.OnSingleClick = new Action(() =>
        {
            Item i = itemButton.refObject as Item;
            BigBoss.Player.pickUpItem(i, currentChest.Location);
            BigBoss.Gooey.OpenGroundGUI(currentChest);
            BigBoss.Gooey.OpenInventoryGUI();
        });
    }

    void CreateEatButton(Item item, ScrollingGrid grid)
    {
        GUIButton itemButton = CreateObjectButton(item, grid, "Eat Item");
        itemButton.OnSingleClick = new Action(() =>
        {
            Item i = itemButton.refObject as Item;
            if (!i.OnGround)
            {
                BigBoss.Player.eatItem(i);
                BigBoss.Gooey.OpenInventoryGUI();
                BigBoss.Gooey.RegenItemInfoGUI(i);
            }
        });
    }

    internal void CheckChestDistance()
    {
        if (currentChest != null)
        {
            if (!BigBoss.Gooey.currentChest.CheckDistance())
            {
                BigBoss.Gooey.currentChest = null;
                BigBoss.Gooey.OpenGroundGUI(null);
            }
        }
    }

    internal class TextPop
    {
        public string str;
        public Vector3 pos;
        public Color col;

        public TextPop(string str, Vector3 pos, Color col)
        {
            this.str = str;
            this.pos = pos;
            this.col = col;
        }

        public void Display()
        {
            GameObject go = Instantiate(BigBoss.Gooey.textPopPrefab, Camera.main.WorldToViewportPoint(pos), Quaternion.identity) as GameObject;
            GUIText textComp = (GUIText)go.GetComponent<GUIText>();
            textComp.text = str;
            textComp.material.color = col;
        }
    }
    #endregion
}
