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

    //SAFETIES WHICH ANY SCRIPT CAN CHECK THROUGH BIGBOSS:
    //public bool confirmationWindowOpen;
    //public bool tooltipOpen;

    public GameObject debugText;
    private Queue<TextPop> textPopList = new Queue<TextPop>();
    public GameObject textPopPrefab;

    #region Clean Inventory Variables
    //public bool isInventoryOpen = false;
    //Panels:
    //private UIPanel inventoryPanel;  //this is currently required to stay at 0,0,0 (camera screen center)

    //Publicly populated variables from scene
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

    //Sprites:
    private UISprite inventoryFrameSprite;
    //Anims
    //public TweenPosition invPanelTweenPos;

    //Misc NGUI Integration:
    private ItemStorage inventoryStorageScript;
    //public UISprite[] inventoryIconArray;

    internal ItemChest currentChest;

    internal bool categoryDisplay = false;
    internal bool displayItem = false;
    internal string category = "";
    public bool displayGUI;
    #endregion

    void Start()
    {
        StartCoroutine(Display());
        //StartCoroutine(DisplayInventory());
    }

    public void Initialize()
    {
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
                //string line7MouseInfo = "\nMouse Stats:    X Location: " + BigBoss.PlayerInput.mouseLocation.x + "  -  Y Location: " +  BigBoss.PlayerInput.mouseLocation.y + "  -  X Axis: " + BigBoss.PlayerInput.horizontalMouseAxis + "  - Y Axis: " + BigBoss.PlayerInput.verticalMouseAxis;
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

    #region INVENTORY
    public void InventoryGUICaptureReferences()//To refresh references when game starts or upon inventory max size change:
    {
        /*
        //NEED TO PUT A FAILSAFE HERE IN CASE ONE RETURNS NULL!!!!!!!
        try
        {
            inventoryPanel = GameObject.Find("Inventory_Panel_Background").GetComponent("UIPanel") as UIPanel;
            inventoryStorageScript = inventoryPanel.gameObject.GetComponentInChildren<ItemStorage>();
            invPanelTweenPos = (TweenPosition)inventoryPanel.GetComponent("TweenPosition") as TweenPosition;
            inventoryFrameSprite = GameObject.Find("Sprite_InventoryFrame").GetComponent("UISprite") as UISprite;
        }
        catch (Exception ex)
        {
            Debug.Log("GUI Exception: " + ex);
        }

        //Icon array to switch on visuals, initializing to size:
        //inventoryIconArray = new UISprite[itemStorageScript.maxItemCount];
        //inventoryIconArray = itemStorageScript.gameObject.GetComponentsInChildren<UISprite>();

        //Debug:
        //Debug.Log(inventoryIconArray.Length);
         */
    }
    /*
    public void ToggleInventoryPanel()
    {
        if (isInventoryOpen)
        {
            //Debug.Log("Calling CloseInventory()...");
            //CloseInventoryUI();
        }
        else
        {
            //Debug.Log("Calling OpenInventory()...");
           // OpenInventoryUI();
        }
    }

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

    #endregion

    #region KCInventory

    public void ClearAllInventorySprites()
    {
        foreach (ItemSlot sl in inventoryStorageScript.InventorySlots)
        {
            sl.icon.enabled = false;
            //Debug.Log(sl.icon.spriteName);
        }
    }

    public void AddItemToGUIInventory(Item item, int count)
    {
        ItemSlot slot = inventoryStorageScript.InventorySlots[count];
        slot.icon.spriteName = "berry02"; //DOUBLE CHECK IF THE ZERO INDEXING IS CORRECT!!!!!!
        //Quantity Label:
        slot.label.enabled = true;
        slot.label.text = count.ToString();

    }

    public void SeedInventory(List<Item> invList)
    {
        //		foreach (UISprite icon in inventoryIconArray)
        //		{
        //			
        //		}
        Debug.Log("Seeding inventory..." + invList.Count + " items in player's list.");
        foreach (Item item in invList)
        {
            //Debugging:
            Debug.Log(item.Name);
            string iconToSetString = item.ModelTexture.ToString();

            //inventoryStorageScript.items.Add(item);
            Debug.Log(item + " added.");
        }
        Debug.Log("Seed Inventory end - " + inventoryStorageScript.items.Count + " items in player's UIStorage.");
    }
    #endregion

    #region UPDATING OF VARIOUS NGUI ELEMENTS - GENERALLY DRIVEN FROM CODE ELSEWHERE IN THE PROJECT
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
        if (textPopList.Count > 20)
        {
            //skip entries when the count gets too high
            for (int i = 0; i < 15; i++)
            {
                textPopList.Dequeue();
            }
        }
        else if (textPopList.Count > 0)
        {
            textPopList.Dequeue().Display();
        }
    }

    IEnumerator Display()
    {
        while (true)
        {
            DisplayTextPops();
            yield return new WaitForSeconds(.25f);
        }
    }

    internal void RegenInventoryGUI()
    {
        if (displayGUI)
        {
            InventoryLabel.SetActive(true);
            inventoryClip.gameObject.SetActive(true);
            itemInfoClip.gameObject.SetActive(true);
            itemActionClip.gameObject.SetActive(true);
            inventoryGrid.Clear();
            if (!categoryDisplay)
            {
                foreach (InventoryCategory ic in BigBoss.Player.inventory.Values)
                {

                    CreateCategoryButton(ic, inventoryGrid, inventoryClipDrag);
                }
            }
            else
            {
                InventoryCategory ic = BigBoss.Player.inventory[category];
                foreach (ItemList itemList in ic.Values)
                {
                    if (itemList.Count > 0)
                    {
                        CreateItemButton(itemList, inventoryGrid, inventoryClipDrag);
                    }
                }
                CreateBackLabel(inventoryGrid, inventoryClipDrag);
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
        if (displayGUI && chest != null)
        {
            currentChest = chest;
            List<Item> items = chest.items;
            GroundLabel.SetActive(true);
            groundClip.gameObject.SetActive(true);
            itemInfoClip.gameObject.SetActive(true);
            itemActionClip.gameObject.SetActive(true);
            groundGrid.Clear();
            foreach (Item item in items)
            {
                CreateItemButton(item, groundGrid, groundClipDrag);
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

    internal void RegenItemInfoGUI(ItemList item)
    {
        if (displayGUI)
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

    GUIButton CreateButton(string buttonName = "Button")
    {
        GameObject button = Instantiate(ButtonPrefab) as GameObject;
        button.name = buttonName;
        return button.GetComponent<GUIButton>();
    }

    GUIButton CreateButton(KGrid parent, string buttonName = "Button", string buttonText = null)
    {
        if (buttonText == null) buttonText = buttonName;
        GUIButton button = CreateButton(buttonName);
        button.Text = buttonText;
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

    void CreateTextButton(string s, KGrid grid, UIDraggablePanel panel)
    {
        GameObject go = Instantiate(InvItemPrefab) as GameObject;
        go.transform.parent = grid.transform;
        go.name = s;
        UIDragPanelContents uiDrag = go.GetComponent<UIDragPanelContents>() as UIDragPanelContents;
        uiDrag.draggablePanel = panel;
        go.transform.localScale = new Vector3(1f, 1f, 1f);
        go.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    void CreateItemButton(ItemList itemList, KGrid grid, UIDraggablePanel panel)
    {
        string buttonText;
        if (itemList.Count > 1)
        {
            buttonText = itemList.id + " (" + itemList.Count + ")";
        }
        else { buttonText = itemList.id; }
        GUIButton itemButton = CreateObjectButton(itemList, grid, itemList.id, buttonText);
        itemButton.OnSingleClick = new Action(() =>
        {
            if ((itemButton.refObject as ItemList).Count > 0)
            {
                BigBoss.Gooey.displayItem = true;
                BigBoss.Gooey.RegenItemInfoGUI(itemButton.refObject as ItemList);
            }
        });
        itemButton.UIDragPanel.draggablePanel = panel;
    }

    void CreateItemButton(Item item, KGrid grid, UIDraggablePanel panel)
    {
        ItemList itemList = new ItemList(item.Name);
        if (grid == groundGrid) { itemList.onGround = true; }
        else if (grid == inventoryGrid) { itemList.onGround = false; }
        itemList.Add(item);
        CreateItemButton(itemList, grid, panel);
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

    void GenerateItemInfo(ItemList item)
    {
        if (item.Count > 0)
        {
            foreach (KeyValuePair<string, string> kvp in item[0].GetGUIDisplays())
            {
                CreateTextButton(kvp.Key + ": " + kvp.Value, itemInfoGrid, itemInfoClipDrag);
            }
            CreateBackLabel(itemInfoGrid, itemInfoClipDrag);
            this.itemInfoClipDrag.ResetPosition();
            itemInfoClip.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            itemInfoClip.clipRange = new Vector4(300, -400, 200, 800);
            this.itemInfoGrid.Reposition();
        }
    }

    void GenerateItemActions(ItemList item)
    {
        if (item.Count > 0)
        {
            CreateEquipButton(item);
            CreateUseButton(item);
            CreateEatButton(item);
            if (item.onGround)
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
    }

    void CreateEquipButton(ItemList item)
    {
        GUIButton itemButton = CreateObjectButton(item, itemActionsGrid, "Equip Item");
        itemButton.OnSingleClick = new Action(() =>
        {
            ItemList itemList = itemButton.refObject as ItemList;
            if (itemList.Count > 0)
            {
                Item itemAlreadyEquipped = BigBoss.Player.getEquippedItems().Find(i => i.Name.Equals(itemList[0].Name));
                if (itemAlreadyEquipped != null)
                {
                    BigBoss.Player.unequipItem(itemAlreadyEquipped);
                    BigBoss.Gooey.RegenItemInfoGUI(itemList);
                }
                else
                {
                    if (itemList.onGround == true)
                    {
                        BigBoss.Player.inventory.Add(itemList[itemList.Count - 1]);
                        BigBoss.Player.Location.Remove(itemList[itemList.Count - 1]);
                        BigBoss.Gooey.GenerateGroundItems(currentChest);
                    }
                    BigBoss.Player.equipItem(itemList[itemList.Count - 1]);
                    BigBoss.Gooey.RegenItemInfoGUI(itemList);
                }
            }
        });
        itemButton.UIDragPanel.draggablePanel = itemActionsClipDrag;
    }

    void CreateUseButton(ItemList item)
    {
        GUIButton itemButton = CreateObjectButton(item, itemActionsGrid, "Use Item");
        itemButton.OnSingleClick = new Action(() =>
        {
            ItemList itemList = itemButton.refObject as ItemList;
            if (itemList.Count > 0)
            {
                BigBoss.Player.useItem(itemList[itemList.Count - 1]);
                BigBoss.Gooey.RegenItemInfoGUI(itemList);
            }
        });
        itemButton.UIDragPanel.draggablePanel = itemActionsClipDrag;
    }

    void CreateDropButton(ItemList item)
    {
        GUIButton itemButton = CreateObjectButton(item, itemActionsGrid, "Drop Item");
        itemButton.OnSingleClick = new Action(() =>
        {
            ItemList itemList = itemButton.refObject as ItemList;
            if (itemList.Count > 0)
            {
                Item dropped = itemList[itemList.Count - 1];
                BigBoss.Player.inventory.Remove(dropped);
                BigBoss.Player.Location.Put(dropped);
                BigBoss.Gooey.RegenInventoryGUI();
                BigBoss.Gooey.GenerateGroundItems(currentChest);
            }
        });
        itemButton.UIDragPanel.draggablePanel = itemActionsClipDrag;
    }

    void CreatePickUpButton(ItemList item)
    {
        GUIButton itemButton = CreateObjectButton(item, itemActionsGrid, "Pick Up Item");
        itemButton.OnSingleClick = new Action(() =>
        {
            ItemList itemList = itemButton.refObject as ItemList;
            if (itemList.Count > 0)
            {
                Item picked = itemList[itemList.Count - 1];
                BigBoss.Player.inventory.Add(picked);
                if (BigBoss.Gooey.currentChest.Remove(picked))
                {
                    currentChest = null;
                }
                BigBoss.Gooey.GenerateGroundItems(currentChest);
                BigBoss.Gooey.RegenInventoryGUI();
            }
        });
        itemButton.UIDragPanel.draggablePanel = itemActionsClipDrag;
    }

    void CreateEatButton(ItemList item)
    {
        GUIButton itemButton = CreateObjectButton(item, itemActionsGrid, "Eat Item");
        itemButton.OnSingleClick = new Action(() =>
        {
            ItemList itemList = itemButton.refObject as ItemList;
            if (itemList.Count > 0)
            {
                if (itemList.onGround == true)
                {
                    BigBoss.Player.Location.Remove(itemList[itemList.Count - 1]);
                    BigBoss.Gooey.GenerateGroundItems(currentChest);
                }
                BigBoss.Player.eatItem(itemList[itemList.Count - 1]);
                BigBoss.Gooey.RegenItemInfoGUI(itemList);
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
            GameObject go = Instantiate(BigBoss.Gooey.textPopPrefab, Camera.mainCamera.WorldToViewportPoint(pos), Quaternion.identity) as GameObject;
            GUIText textComp = (GUIText)go.GetComponent<GUIText>();
            textComp.text = str;
            textComp.material.color = col;
        }
    }
    #endregion
}
