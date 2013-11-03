using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GUIManager : MonoBehaviour, IManager
{
    // Disable unassigned warnings
    #pragma warning disable 414, 219

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
    public UIPanel itemInfoClip;
    public UIPanel itemActionClip;

    //grids
    public UIGrid inventoryGrid;
    public UIGrid itemInfoGrid;
    public UIGrid itemActionsGrid;
    public GameObject InvItemPrefab;

    //Clip panels
    public UIDraggablePanel inventoryClipDrag;
    public UIDraggablePanel itemInfoClipDrag;
    public UIDraggablePanel itemActionsClipDrag;
    public UIFont font;

    //Sprites:
    private UISprite inventoryFrameSprite;
    //Anims
    //public TweenPosition invPanelTweenPos;

    //Misc NGUI Integration:
    private ItemStorage inventoryStorageScript;
    //public UISprite[] inventoryIconArray;

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
            Debug.Log(item.name);
            string iconToSetString = item.ModelTexture.ToString();

            //inventoryStorageScript.items.Add(item);
            Debug.Log(item + " added.");
        }
        Debug.Log("Seed Inventory end - " + inventoryStorageScript.items.Count + " items in player's UIStorage.");
    }
    #endregion

    #region KCInventory

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
            ClearPriorGrid(inventoryGrid);
            this.inventoryGrid.sorted = false;
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
                    CreateItemButton(itemList, inventoryGrid, inventoryClipDrag);
                }
                CreateBackLabel(inventoryGrid, inventoryClipDrag);
            }
            this.inventoryClipDrag.ResetPosition();
            inventoryClip.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            inventoryClip.clipRange = new Vector4(100, -400, 200, 797);
            this.inventoryGrid.Reposition();
        }
    }

    internal void RegenItemInfoGUI(ItemList item)
    {
        if (displayGUI)
        {
            ClearPriorGrid(itemInfoGrid);
            this.itemInfoGrid.sorted = false;
            if (displayItem)
            {
                GenerateItemInfo(item);
                GenerateItemActions(item);
            }
            else
            {
                RegenInventoryGUI();
                ClearPriorGrid(itemInfoGrid);
            }
        }
    }

    internal void ClearPriorGrid(UIGrid grid)
    {
        foreach (Transform child in grid.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void CreateBackLabel(UIGrid grid, UIDraggablePanel panel)
    {
        GameObject go = Instantiate(InvItemPrefab) as GameObject;
        go.transform.parent = grid.transform;
        go.name = "BackButton";
        GUIBackLabel guiBackLabel = go.AddComponent<GUIBackLabel>();
        guiBackLabel.label = NGUITools.AddWidget<UILabel>(go);
        guiBackLabel.label.text = "Back";
        guiBackLabel.label.font = font;
        //guiBackLabel.label.depth = 4;
        guiBackLabel.label.MakePixelPerfect();
        UIDragPanelContents uiDragBackLabel = go.GetComponent<UIDragPanelContents>() as UIDragPanelContents;
        uiDragBackLabel.draggablePanel = panel;
        go.transform.localScale = new Vector3(1f, 1f, 1f);
        go.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    void CreateTextButton(string s, UIGrid grid, UIDraggablePanel panel)
    {
        GameObject go = Instantiate(InvItemPrefab) as GameObject;
        go.transform.parent = grid.transform;
        go.name = s;
        GUILabel guiItem = go.AddComponent<GUILabel>();
        guiItem.text = s;
        guiItem.label = NGUITools.AddWidget<UILabel>(go);
        guiItem.label.text = s;
        guiItem.label.font = font;
        //guiItem.label.depth = 4;
        guiItem.label.MakePixelPerfect();
        UIDragPanelContents uiDrag = go.GetComponent<UIDragPanelContents>() as UIDragPanelContents;
        uiDrag.draggablePanel = panel;
        go.transform.localScale = new Vector3(1f, 1f, 1f);
        go.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    void CreateItemButton(ItemList itemList, UIGrid grid, UIDraggablePanel panel)
    {
        GameObject go = Instantiate(InvItemPrefab) as GameObject;
        go.transform.parent = grid.transform;
        go.name = itemList.id;
        GUIItem guiItem = go.AddComponent<GUIItem>();
        guiItem.item = itemList;
        guiItem.label = NGUITools.AddWidget<UILabel>(go);
        guiItem.label.text = itemList.id + " (" + itemList.Count + ")"; //string builder maybe?
        guiItem.label.font = font;
        //guiItem.label.depth = 4;
        guiItem.label.MakePixelPerfect();
        UIDragPanelContents uiDrag = go.GetComponent<UIDragPanelContents>() as UIDragPanelContents;
        uiDrag.draggablePanel = panel;
        go.transform.localScale = new Vector3(1f, 1f, 1f);
        go.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    void CreateCategoryButton(InventoryCategory ic, UIGrid grid, UIDraggablePanel panel)
    {
        GameObject go = Instantiate(InvItemPrefab) as GameObject;
        go.transform.parent = grid.transform;
        go.name = ic.id;
        GUIInventoryCategory guiIC = go.AddComponent<GUIInventoryCategory>();
        guiIC.category = ic;
        guiIC.label = NGUITools.AddWidget<UILabel>(go);
        guiIC.label.text = ic.id;
        guiIC.label.font = font;
        //guiIC.label.depth = 4;
        guiIC.label.MakePixelPerfect();
        UIDragPanelContents uiDrag = go.GetComponent<UIDragPanelContents>() as UIDragPanelContents;
        uiDrag.draggablePanel = panel;
        go.transform.localScale = new Vector3(1f, 1f, 1f);
        go.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    void GenerateItemInfo(ItemList item)
    {
        foreach (KeyValuePair<string, Field> kvp in item[0].map)
        {
            CreateTextButton(kvp.Key + ": " + kvp.Value.ToString(), itemInfoGrid, itemInfoClipDrag);
        }
        CreateBackLabel(itemInfoGrid, itemInfoClipDrag);
        this.itemInfoClipDrag.ResetPosition();
        itemInfoClip.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        itemInfoClip.clipRange = new Vector4(400, -400, 400, 800);
        this.itemInfoGrid.Reposition();
    }

    void GenerateItemActions(ItemList item)
    {
        CreateEquipButton(item);
        CreateUseButton(item);
        CreateDropButton(item);
        CreateEatButton(item);
        this.itemActionsClipDrag.ResetPosition();
        itemActionClip.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        itemActionClip.clipRange = new Vector4(1000, -400, 400, 800);
        this.itemActionsGrid.Reposition();
    }

    void CreateEquipButton(ItemList item)
    {
        GameObject go = Instantiate(InvItemPrefab) as GameObject;
        go.transform.parent = itemActionsGrid.transform;
        go.name = item[0].Name;
        GUIEquipItem guiItem = go.AddComponent<GUIEquipItem>();
        guiItem.item = item;
        guiItem.label = NGUITools.AddWidget<UILabel>(go);
        guiItem.label.text = "Equip Item";
        guiItem.label.font = font;
        guiItem.label.MakePixelPerfect();
        UIDragPanelContents uiDrag = go.GetComponent<UIDragPanelContents>() as UIDragPanelContents;
        uiDrag.draggablePanel = inventoryClipDrag;
        go.transform.localScale = new Vector3(1f, 1f, 1f);
        go.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    void CreateUseButton(ItemList item)
    {
        GameObject go = Instantiate(InvItemPrefab) as GameObject;
        go.transform.parent = itemActionsGrid.transform;
        go.name = item[0].Name;
        GUIUseItem guiItem = go.AddComponent<GUIUseItem>();
        guiItem.item = item;
        guiItem.label = NGUITools.AddWidget<UILabel>(go);
        guiItem.label.text = "Use Item";
        guiItem.label.font = font;
        guiItem.label.MakePixelPerfect();
        UIDragPanelContents uiDrag = go.GetComponent<UIDragPanelContents>() as UIDragPanelContents;
        uiDrag.draggablePanel = inventoryClipDrag;
        go.transform.localScale = new Vector3(1f, 1f, 1f);
        go.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    void CreateDropButton(ItemList item)
    {
        GameObject go = Instantiate(InvItemPrefab) as GameObject;
        go.transform.parent = itemActionsGrid.transform;
        go.name = item[0].Name;
        GUIDropItem guiItem = go.AddComponent<GUIDropItem>();
        guiItem.item = item;
        guiItem.label = NGUITools.AddWidget<UILabel>(go);
        guiItem.label.text = "Drop Item";
        guiItem.label.font = font;
        guiItem.label.MakePixelPerfect();
        UIDragPanelContents uiDrag = go.GetComponent<UIDragPanelContents>() as UIDragPanelContents;
        uiDrag.draggablePanel = inventoryClipDrag;
        go.transform.localScale = new Vector3(1f, 1f, 1f);
        go.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    void CreateEatButton(ItemList item)
    {
        GameObject go = Instantiate(InvItemPrefab) as GameObject;
        go.transform.parent = itemActionsGrid.transform;
        go.name = item[0].Name;
        GUIEatItem guiItem = go.AddComponent<GUIEatItem>();
        guiItem.item = item;
        guiItem.label = NGUITools.AddWidget<UILabel>(go);
        guiItem.label.text = "Eat Item";
        guiItem.label.font = font;
        guiItem.label.MakePixelPerfect();
        UIDragPanelContents uiDrag = go.GetComponent<UIDragPanelContents>() as UIDragPanelContents;
        uiDrag.draggablePanel = inventoryClipDrag;
        go.transform.localScale = new Vector3(1f, 1f, 1f);
        go.transform.localPosition = new Vector3(0f, 0f, 0f);
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

    //public void PlayerTouched()
    //{
    //    ToggleInventoryPanel();
    //    ClearAllInventorySprites();
    //}
}
