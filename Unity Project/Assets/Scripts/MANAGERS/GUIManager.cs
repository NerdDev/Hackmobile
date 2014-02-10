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

    internal bool categoryDisplay = false;
    internal bool displayItem = false;
    internal string category = "";
    public bool displayInventory;
    #endregion

    #region Publicly populated variables from scene
    //Clip panels
    public UIPanel inventoryClip;
    public UIPanel groundClip;
    public UIPanel itemInfoClip;
    public UIPanel itemActionClip;

    //grids
    public KGrid inventoryGrid;
    public KGrid groundGrid;
    public KGrid itemInfoGrid;
    public KGrid itemActionsGrid;

    //Prefabs
    public GameObject InvItemPrefab;
    public GameObject ButtonPrefab;
    public GameObject ChestPrefab;

    //Clip panels
    public UIDraggablePanel inventoryClipDrag;
    public UIDraggablePanel groundClipDrag;
    public UIDraggablePanel itemInfoClipDrag;
    public UIDraggablePanel itemActionsClipDrag;
    public UIFont font;

    //Misc
    public GameObject InventoryLabel;
    public GameObject GroundLabel;
    public GameObject debugText;
    public GameObject textPopPrefab;
    public Light light;
    public Texture cookie;
    public GUITexture LoadImage;

    //Cameras
    public Camera GUICam;
    public Camera HeroCam;
    #endregion

    void Start()
    {
        StartCoroutine(Display());
    }

    public void Initialize()
    {
    }

    public void OpenMenuGUI()
    {
        //prep, close other GUI elements
        displayInventory = false;
        RegenInventoryGUI();
        GenerateGroundItems(null);
        //LoadImage.gameObject.SetActive(false);

        List<GameObject> buttons = new List<GameObject>();
        GUIButton startButton = CreateButton("Start");
        buttons.Add(startButton.gameObject);
        FixButton(startButton);
        startButton.transform.localScale = new Vector3(.01f, .01f, .01f);
        startButton.transform.localPosition = new Vector3(0f, .5f, 0f);
        startButton.OnSingleClick = new Action(() =>
        {
            StartCoroutine(BigBoss.Start.StartGame(buttons));
        });

        GUIButton startButton2 = CreateButton("Disco Start");
        buttons.Add(startButton2.gameObject);
        FixButton(startButton2);
        startButton2.transform.localScale = new Vector3(.01f, .01f, .01f);
        startButton2.transform.localPosition = new Vector3(0f, -.5f, 0f);
        startButton2.OnSingleClick = new Action(() =>
        {
            light.color = Color.white;
            light.cookie = cookie;
            StartCoroutine(BigBoss.Start.StartGame(buttons));
        });
    }

    public void OpenInventoryGUI()
    {
        displayInventory = true;
        RegenInventoryGUI();
        GenerateGroundItems(currentChest);
    }

    public void DisplayLoading()
    {
        LoadImage.enabled = true;
    }

    public void CloseLoading()
    {
        LoadImage.enabled = false;
    }

    //Debugging Coroutine
    private IEnumerator ShowDebugInfo()
    {

        GUIText textComp;
        //If text doesn't exist, create one:
        if (GameObject.Find("DEBUG TEXT") == null)
        {
            debugText = new GameObject("DEBUG TEXT");
            GUIText text = debugText.AddComponent("GUIText") as GUIText;
            textComp = text;
            textComp.fontSize = 14;
            Vector3 placeToPutIt = new Vector3(0, 1, 1);
            debugText.transform.position = placeToPutIt;

        }
        else textComp = GameObject.Find("DEBUG TEXT").GetComponent("GUIText") as GUIText;

        while (this.enabled == true)//Infinite Loop:
        {
            /*
            if (BigBoss.GameStateManager.DebugMode	== true)//Only Calculate this stuff in Debug Mode:
            {
                textComp.enabled = true;
				
                //This is only on top because letting this run first frame will throw a nullref due to audio
                string line1DebugKey = "Tilde-Backquote Key Toggles Debug Mode\n";
                string line2TotalTimeRunning = BigBoss.TimeKeeper.TotalTimeSinceStartup;
                string line3CurrentScene = "\nCurrent Level/Scene: " + Application.loadedLevelName;				
                string line4CurrentGameState = "\nCurrent Game State: " + BigBoss.GameStateManager.State;
                string line5currentTime = "\n" + BigBoss.TimeKeeper.RealComputerTimeNeat;
                string line6totalTimePlayed = "\n" + BigBoss.TimeKeeper.TotalTimePlayedNeat;
                //string line7MouseInfo = "\nMouse Stats:    X GridSpace: " + BigBoss.PlayerInput.mouseLocation.x + "  -  Y GridSpace: " +  BigBoss.PlayerInput.mouseLocation.y + "  -  X Axis: " + BigBoss.PlayerInput.horizontalMouseAxis + "  - Y Axis: " + BigBoss.PlayerInput.verticalMouseAxis;
                string line7MouseInfo = "Center Screen Space Point: " + BigBoss.PlayerInput.centerPointInScreenSpace + ", Current Mouse Screen Position: " + ((Vector2)Input.mousePosition);
                //string line8audioInfo;
				
				
                textComp.text = line1DebugKey + line2TotalTimeRunning + line3CurrentScene + line4CurrentGameState + line5currentTime + line6totalTimePlayed + line7MouseInfo;	
				
                yield return null;
                //yield return new WaitForSeconds (.05f);
            }
                */
            //else textComp.enabled = false;
            yield return null;
        }
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

    #region GUI
    public void UpdateHealthBar()
    {
        //		HUDplayerHealthBar.sliderValue = (float)BigBoss.PlayerInfo.stats.CurrentHealth/(float)BigBoss.PlayerInfo.stats.MaxHealth;	
        //		HUDHealthBarNumberLabel.text = BigBoss.PlayerInfo.stats.CurrentHealth + " / " + BigBoss.PlayerInfo.stats.MaxHealth;
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
        BigBoss.Gooey.CreateTextPop(this.gameObject.transform.position + Vector3.up * .75f, hunger.ToString() + "!", color);
    }

    public void CreateTextPop(Vector3 worldPosition, string message) //no color arg version
    {
        CreateTextPop(worldPosition, message, Color.white);
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

    internal void RegenInventoryGUI()
    {
        if (displayInventory)
        {
            InventoryLabel.SetActive(true);
            inventoryClip.gameObject.SetActive(true);
            itemInfoClip.gameObject.SetActive(true);
            itemActionClip.gameObject.SetActive(true);
            inventoryGrid.Clear();
            if (!categoryDisplay)
            {
                foreach (InventoryCategory ic in BigBoss.Player.Inventory.Values)
                {

                    CreateCategoryButton(ic, inventoryGrid, inventoryClipDrag);
                }
            }
            else
            {
                InventoryCategory ic;
                if (BigBoss.Player.Inventory.TryGetValue(category, out ic))
                {
                    foreach (Item item in ic.Values)
                    {
                        CreateItemButton(item, inventoryGrid, inventoryClipDrag);
                    }
                    CreateBackLabel(inventoryGrid, inventoryClipDrag);
                }
            }
            this.inventoryClipDrag.ResetPosition();
            inventoryClip.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            inventoryClip.clipRange = new Vector4(100, -210, 200, 325);
            this.inventoryGrid.Reposition();
        }
        else
        {
            InventoryLabel.SetActive(false);
            inventoryGrid.Clear();
            itemInfoGrid.Clear();
            itemActionsGrid.Clear();
            inventoryClip.gameObject.SetActive(false);
            itemInfoClip.gameObject.SetActive(false);
            itemActionClip.gameObject.SetActive(false);
        }
    }

    internal void GenerateGroundItems(ItemChest chest)
    {
        if (displayInventory && chest != null)
        {
            currentChest = chest;
            Inventory inv = chest.Location.inventory;
            GroundLabel.SetActive(true);
            groundClip.gameObject.SetActive(true);
            itemInfoClip.gameObject.SetActive(true);
            itemActionClip.gameObject.SetActive(true);
            groundGrid.Clear();
            foreach (InventoryCategory ic in inv.Values)
            {
                foreach (Item item in ic.Values)
                {
                    CreateItemButton(item, groundGrid, groundClipDrag);
                }
            }
            CreateCloseLabel(groundGrid, groundClipDrag);
            this.groundClipDrag.ResetPosition();
            groundClip.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            groundClip.clipRange = new Vector4(100, -610, 190, 375);
            this.groundGrid.Reposition();
        }
        else
        {
            GroundLabel.SetActive(false);
            groundGrid.Clear();
            itemInfoGrid.Clear();
            itemActionsGrid.Clear();
            groundClip.gameObject.SetActive(false);
            itemInfoClip.gameObject.SetActive(false);
            itemActionClip.gameObject.SetActive(false);
        }
    }

    private void CreateCloseLabel(KGrid grid, UIDraggablePanel panel)
    {
        GUIButton button = CreateButton(grid, "CloseButton", "Close");
        button.OnSingleClick = new Action(() =>
        {
            BigBoss.Gooey.category = "";
            BigBoss.Gooey.categoryDisplay = false;
            BigBoss.Gooey.GenerateGroundItems(null);
            BigBoss.Gooey.displayItem = false;
            BigBoss.Gooey.RegenItemInfoGUI(null);
        });
        button.UIDragPanel.draggablePanel = panel;
    }

    internal void RegenItemInfoGUI(Item item)
    {
        if (displayInventory)
        {
            itemInfoGrid.Clear();
            itemActionsGrid.Clear();
            if (displayItem)
            {
                GenerateItemInfo(item);
                GenerateItemActions(item);
            }
            else
            {
                RegenInventoryGUI();
                itemInfoGrid.Clear();
                itemActionsGrid.Clear();
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

    GUIButton CreateButton(KGrid parent, string buttonName = "Button", string buttonText = null)
    {
        if (buttonText == null) buttonText = buttonName;
        GUIButton button = CreateButton(buttonName, buttonText);
        parent.AddButton(button);
        FixButton(button);
        return button.GetComponent<GUIButton>();
    }

    GUIButton CreateObjectButton(System.Object o, KGrid parent, string buttonName = "Button", string buttonText = null)
    {
        GUIButton button = CreateButton(parent, buttonName, buttonText);
        button.refObject = o;
        return button;
    }

    void FixButton(GUIButton button)
    {
        button.label.MakePixelPerfect();
        button.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        button.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    void CreateBackLabel(KGrid grid, UIDraggablePanel panel)
    {
        GUIButton button = CreateButton(grid, "BackButton", "Back");
        button.OnSingleClick = new Action(() =>
        {
            BigBoss.Gooey.category = "";
            BigBoss.Gooey.categoryDisplay = false;
            BigBoss.Gooey.RegenInventoryGUI();
            BigBoss.Gooey.displayItem = false;
            BigBoss.Gooey.RegenItemInfoGUI(null);
        });
        button.UIDragPanel.draggablePanel = panel;
    }

    void CreateItemButton(Item item, KGrid grid, UIDraggablePanel panel)
    {
        if (grid == groundGrid) { item.OnGround = true; }
        else if (grid == inventoryGrid) { item.OnGround = false; }
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
        itemButton.UIDragPanel.draggablePanel = panel;
    }

    void CreateCategoryButton(InventoryCategory ic, KGrid grid, UIDraggablePanel panel)
    {
        GUIButton categoryButton = CreateObjectButton(ic, grid, ic.id);
        categoryButton.OnSingleClick = new Action(() =>
        {
            BigBoss.Gooey.categoryDisplay = true;
            BigBoss.Gooey.category = (categoryButton.refObject as InventoryCategory).id;
            BigBoss.Gooey.RegenInventoryGUI();
        });
        categoryButton.UIDragPanel.draggablePanel = panel;
    }

    void GenerateItemInfo(Item item)
    {
        if (item.Count > 0)
        {
            foreach (KeyValuePair<string, string> kvp in item.GetGUIDisplays())
            {
                //CreateTextButton(kvp.Key + ": " + kvp.Value, itemInfoGrid, itemInfoClipDrag);
            }
            CreateBackLabel(itemInfoGrid, itemInfoClipDrag);
            this.itemInfoClipDrag.ResetPosition();
            itemInfoClip.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            itemInfoClip.clipRange = new Vector4(300, -400, 200, 800);
            this.itemInfoGrid.Reposition();
        }
    }

    void GenerateItemActions(Item item)
    {
        if (!item.itemFlags[ItemFlags.IS_EQUIPPED])
        {
            CreateEquipButton(item);
        }
        else
        {
            CreateUnEquipButton(item);
        }
        CreateUseButton(item);
        CreateEatButton(item);
        if (item.OnGround)
        {
            CreatePickUpButton(item);
        }
        else
        {
            CreateDropButton(item);
        }
        this.itemActionsClipDrag.ResetPosition();
        itemActionClip.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        itemActionClip.clipRange = new Vector4(500, -400, 200, 800);
        this.itemActionsGrid.Reposition();
    }

    void CreateEquipButton(Item item)
    {
        GUIButton itemButton = CreateObjectButton(item, itemActionsGrid, "Equip Item");
        itemButton.OnSingleClick = new Action(() =>
        {
            Item i = itemButton.refObject as Item;
            if (i.OnGround == true)
            {
                BigBoss.Player.Inventory.Add(i);
                BigBoss.Player.GridSpace.Remove(i);
                BigBoss.Gooey.GenerateGroundItems(currentChest);
            }
            BigBoss.Player.equipItem(i);
            BigBoss.Gooey.RegenItemInfoGUI(i);
        });
        itemButton.UIDragPanel.draggablePanel = itemActionsClipDrag;
    }

    void CreateUnEquipButton(Item item)
    {
        GUIButton itemButton = CreateObjectButton(item, itemActionsGrid, "UnEquip Item");
        itemButton.OnSingleClick = new Action(() =>
        {
            Item i = itemButton.refObject as Item;
            Item itemAlreadyEquipped = BigBoss.Player.getEquippedItems().Find(items => items.Name.Equals(i.Name));
            if (itemAlreadyEquipped != null)
            {
                BigBoss.Player.unequipItem(itemAlreadyEquipped);
                BigBoss.Gooey.RegenItemInfoGUI(itemAlreadyEquipped);
            }
        });
        itemButton.UIDragPanel.draggablePanel = itemActionsClipDrag;
    }

    void CreateUseButton(Item item)
    {
        GUIButton itemButton = CreateObjectButton(item, itemActionsGrid, "Use Item");
        itemButton.OnSingleClick = new Action(() =>
        {
            Item i = itemButton.refObject as Item;
            BigBoss.Player.useItem(i);
            BigBoss.Gooey.RegenItemInfoGUI(i);
        });
        itemButton.UIDragPanel.draggablePanel = itemActionsClipDrag;
    }

    void CreateDropButton(Item item)
    {
        GUIButton itemButton = CreateObjectButton(item, itemActionsGrid, "Drop Item");
        itemButton.OnSingleClick = new Action(() =>
        {
            Item i = itemButton.refObject as Item;
            BigBoss.Player.dropItem(i, BigBoss.Player.GridSpace);
            BigBoss.Gooey.RegenInventoryGUI();
            BigBoss.Gooey.GenerateGroundItems(currentChest);
        });
        itemButton.UIDragPanel.draggablePanel = itemActionsClipDrag;
    }

    void CreatePickUpButton(Item item)
    {
        GUIButton itemButton = CreateObjectButton(item, itemActionsGrid, "Pick Up Item");
        itemButton.OnSingleClick = new Action(() =>
        {
            Item i = itemButton.refObject as Item;
            BigBoss.Player.pickUpItem(i, currentChest.Location);
            BigBoss.Gooey.GenerateGroundItems(currentChest);
            BigBoss.Gooey.RegenInventoryGUI();
        });
        itemButton.UIDragPanel.draggablePanel = itemActionsClipDrag;
    }

    void CreateEatButton(Item item)
    {
        GUIButton itemButton = CreateObjectButton(item, itemActionsGrid, "Eat Item");
        itemButton.OnSingleClick = new Action(() =>
        {
            Item i = itemButton.refObject as Item;
            if (!i.OnGround)
            {
                BigBoss.Player.eatItem(i);
                BigBoss.Gooey.RegenItemInfoGUI(i);
            }
        });
        itemButton.UIDragPanel.draggablePanel = itemActionsClipDrag;
    }

    internal void CheckChestDistance()
    {
        if (currentChest != null)
        {
            if (!BigBoss.Gooey.currentChest.CheckDistance())
            {
                BigBoss.Gooey.currentChest = null;
                BigBoss.Gooey.GenerateGroundItems(null);
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
