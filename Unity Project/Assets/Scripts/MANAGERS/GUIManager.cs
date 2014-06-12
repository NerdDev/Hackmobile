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

    internal bool categoryDisplay = false;
    internal bool displayItem = false;
    internal string category = "";
    public bool displayInventory;
    internal bool DisplayChatGrid = false;

    public Material NormalShaderGridspace;
    public Material GlowShaderGridSpace;

    public SpellCastInfo info;

    List<TargetSpace> TargetSpaces = new List<TargetSpace>();
    int TargetSpaceCount;
    internal class TargetSpace
    {
        public Vector3 loc;
        public GameObject point;
    }
    List<IAffectable> TargetAffectables = new List<IAffectable>();
    int TargetAffectableCount;
    Dictionary<Renderer, Shader[]> originalShaders = new Dictionary<Renderer, Shader[]>();
    HashSet<NPC> Outlining = new HashSet<NPC>();
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
    public Shader OutliningShader;
    public Shader TransparentShader;
    public GameObject InvisibleObject;

    //Cameras
    public Camera GUICam;
    public Camera HeroCam;

    public FOWSystem fow;

    public GameObject PointPrefab;
    public LayerMask MaskForGUI;
    #endregion

    void Start()
    {
        StartCoroutine(Display());
    }

    void Update()
    {
        if (BigBoss.PlayerInput.InputSetting[InputSettings.SPELL_INPUT])
        {
            if (Input.GetMouseButtonDown(0))
            {
                bool hitCamera = true;
                if (UICamera.hoveredObject == null) hitCamera = false;
                if (!hitCamera)
                {
                    RaycastHit hitInfo = new RaycastHit();
                    bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
                    if (hit)
                    {
                        GameObject gObj = hitInfo.collider.gameObject;
                        if (gObj.layer == LayerMask.NameToLayer("Floor") && FOWSystem.instance.IsVisible(hitInfo.point))
                        {
                            if (TargetSpaces.Count < TargetSpaceCount)
                            {
                                Vector3 loc = hitInfo.point;
                                GameObject go = GameObject.Instantiate(PointPrefab, loc, Quaternion.identity) as GameObject;
                                TargetSpaces.Add(new TargetSpace() { point = go, loc = loc });
                                ResetNPCTarget();
                            }
                        }
                    }
                }
            }
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

    internal void SetToSpellCasting(Spell spell)
    {
        Debug.Log("Setting spellcasting mode to: " + spell.ToString());
        if (BigBoss.Player.Stats.CurrentPower > 0)
        {
            currentSpell = spell;
            info = currentSpell.GetCastInfoPrototype(BigBoss.Player);
            if (info.TargetLocations != null && info.TargetLocations.Length > 0)
            {
                TargetSpaceCount = info.TargetLocations.Length;
            }
            else
            {
                TargetSpaceCount = 0;
            }

            if (info.TargetObjects != null && info.TargetObjects.Length > 0)
            {
                TargetAffectableCount = info.TargetObjects.Length;
            }
            else
            {
                TargetAffectableCount = TargetSpaceCount;
            }

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
    }

    private void ResetTargets()
    {
        ResetNPCTarget();
        ResetLocationTargets();
    }

    private void ResetLocationTargets()
    {
        if (TargetSpaces != null)
        {
            for (int i = 0; i < TargetSpaces.Count; i++)
            {
                if (TargetSpaces[i] == null) continue;
                if (TargetSpaces[i].point == null) continue;
                Destroy(TargetSpaces[i].point);
            }
        }
        TargetSpaces.Clear();
    }

    private void ResetNPCTarget()
    {
        foreach (IAffectable ia in TargetAffectables)
        {
            if (ia is NPC)
            {
                NPC n = ia as NPC;
                if (Outlining.Contains(n))
                {
                    ToggleHighlightOff(n);
                    Outlining.Remove(n);
                }
            }
        }
        TargetAffectables.Clear();
    }

    internal void CastSpell()
    {
        if (currentSpell != null)
        {
            if (Math.Min(TargetAffectables.Count, TargetAffectableCount) > 0)
            {
                IAffectable[] targets = new IAffectable[Math.Min(TargetAffectables.Count, TargetAffectableCount)];
                for (int i = 0; i < Math.Min(TargetAffectables.Count, TargetAffectableCount); i++)
                {
                    targets[i] = TargetAffectables[i];
                }
                BigBoss.Player.CastSpell(currentSpell, targets);
            }
            else
            {
                Vector3[] arr = new Vector3[Math.Min(TargetSpaces.Count, TargetSpaceCount)];
                for (int i = 0; i < Math.Min(TargetSpaces.Count, TargetSpaceCount); i++)
                {
                    arr[i] = TargetSpaces[i].loc;
                }
                BigBoss.Player.CastSpell(currentSpell, arr);
            }
        }
        CancelSpell(false);
    }

    public void Target(IAffectable target)
    {
        if (TargetAffectables.Contains(target))
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
        TargetAffectables.Remove(target);
        if (target is NPC)
        {
            NPC n = target as NPC;
            if (Outlining.Contains(n))
            {
                ToggleHighlightOff(n);
                Outlining.Remove(n);
            }
        }
    }

    internal void AddTarget(IAffectable target)
    {
        TargetAffectables.Add(target);
        if (target is NPC)
        {
            NPC n = target as NPC;
            if (!Outlining.Contains(n))
            {
                ToggleHighlightOn(n);
                Outlining.Add(n);
            }
        }
        ResetLocationTargets();
    }
    
    private void ToggleHighlightOn(NPC n)
    {
        GameObject npcObj = n.GO;
        foreach (Renderer r in npcObj.GetComponentsInChildren<Renderer>())
        {
            Material[] materials = r.materials;
            originalShaders.Add(r, new Shader[materials.Length]);
            for (int i = 0; i < materials.Length; i++)
            {
                originalShaders[r][i] = materials[i].shader;
                materials[i].shader = BigBoss.Gooey.OutliningShader;
            }
        }
    }

    private void ToggleHighlightOff(NPC n)
    {
        GameObject npcObj = n.GO;
        foreach (Renderer r in npcObj.GetComponentsInChildren<Renderer>())
        {
            Material[] materials = r.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].shader = originalShaders[r][i];
            }
            originalShaders.Remove(r);
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
