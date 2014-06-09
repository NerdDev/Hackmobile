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
    internal Spell currentSpell;
    internal GUIButton selectedButton;

    internal bool categoryDisplay = false;
    internal bool displayItem = false;
    internal string category = "";
    public bool displayInventory;
    public bool displaySpells;
    internal bool DisplayChatGrid = false;

    public Material NormalShaderGridspace;
    public Material GlowShaderGridSpace;

    public SpellCastInfo info;
    internal HashSet<GridSpace> gridTargets = new HashSet<GridSpace>();
    internal HashSet<IAffectable> spellTargets = new HashSet<IAffectable>();
    #endregion

    #region Publicly populated variables from scene
    public InventoryMenu inventory;
    public ItemMenu itemMenu;
    public SpellMenu spellMenu;
    public GroundMenu ground;
    public TextMenu text;

    public TextGrid ChatGrid;
    public GUIProgressBar HealthBar;
    public GUIProgressBar ManaBar;

    //Prefabs
    public GameObject ButtonPrefab;
    public GameObject ChestPrefab;
    public GameObject LabelPrefab;

    public UIFont font;

    //Misc
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

    public void pickUpItem(Item i)
    {
        BigBoss.Player.pickUpItem(i, ground.chest.Location);
    }

    public void dropItem(Item i)
    {
        BigBoss.Player.dropItem(i, ground.chest.Location);
    }

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
        GUIButton label = button.GetComponent<GUIButton>();
        label.Text = message;
        //label.UIDragPanel.draggablePanel = ChatGrid.DragPanel;
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

    GridSpace[] TargetGrids;
    IAffectable[] TargetAffectables;

    internal void SetToSpellCasting(Spell spell)
    {
        Debug.Log("Setting spellcasting mode to: " + spell.ToString());
        if (BigBoss.Player.Stats.CurrentPower > 0)
        {
            currentSpell = spell;
            info = currentSpell.GetCastInfoPrototype(BigBoss.Player);

            TargetGrids = info.TargetSpaces;
            TargetAffectables = info.TargetObjects;

            BigBoss.PlayerInput.InputSetting[InputSettings.DEFAULT_INPUT] = false;
            BigBoss.PlayerInput.InputSetting[InputSettings.SPELL_INPUT] = true;
            spellMenu.ToggleCastButton(true);
            spellMenu.ToggleCancelButton(true);
        }
        else
        {
            CreateTextPop(BigBoss.PlayerInfo.transform.position, "You do not have enough power to cast " + currentSpell + "!");
            currentSpell = null;
        }
    }

    internal void CancelSpell(bool toggleCancel)
    {
        BigBoss.PlayerInput.InputSetting[InputSettings.DEFAULT_INPUT] = true;
        BigBoss.PlayerInput.InputSetting[InputSettings.SPELL_INPUT] = false;
        currentSpell = null;
        spellMenu.ToggleCastButton(false);
        if (toggleCancel) spellMenu.ToggleCancelButton(false);
        ResetTargets();
        spellTargets.Clear();
    }

    private void ResetTargets()
    {
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

        foreach (GridSpace target in TargetGrids)
        {
            foreach (GameObject block in target.Blocks)
            {
                block.renderer.sharedMaterial = NormalShaderGridspace;
            }
        }
    }

    internal void CastSpell()
    {
        if (currentSpell != null)
        {
            BigBoss.Player.CastSpell(currentSpell, spellTargets.ToArray());
        }
        CancelSpell(false);
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

    GUIButton CreateButton(string buttonName = "Button", string buttonText = null)
    {
        GameObject button = Instantiate(ButtonPrefab) as GameObject;
        GUIButton butt = button.GetComponent<GUIButton>();
        button.name = buttonName;
        if (buttonText == null) buttonText = buttonName;
        butt.Text = buttonText;
        return butt;
    }

    public GUIButton CreateButton(ScrollingGrid grid, string buttonName = "Button", string buttonText = null)
    {
        if (buttonText == null) buttonText = buttonName;
        GUIButton button = CreateButton(buttonName, buttonText);
        // button.UIDragPanel.draggablePanel = grid.DragPanel;
        grid.AddButton(button);
        return button.GetComponent<GUIButton>();
    }

    public GUIButton CreateObjectButton(System.Object o, ScrollingGrid grid, string buttonName = "Button", string buttonText = null)
    {
        GUIButton button = CreateButton(grid, buttonName, buttonText);
        button.refObject = o;
        return button;
    }

    internal void CheckChestDistance()
    {
        if (currentChest != null)
        {
            if (!BigBoss.Gooey.currentChest.CheckDistance())
            {
                BigBoss.Gooey.currentChest = null;
                BigBoss.Gooey.ground.Close();
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
