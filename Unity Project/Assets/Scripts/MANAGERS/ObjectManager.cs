using UnityEngine;
using System;
using System.Collections.Generic;
using XML;
using System.IO;


public class ObjectManager : MonoBehaviour, IManager
{
    public bool Initialized { get; set; }
    const string XMLPath = "Assets/Resources/XML/";
    public NPCDictionary<NPC, NPCInstance> NPCs { get; protected set; }
    public ItemDictionary Items { get; protected set; }
    public Dictionary<string, string> Strings { get; protected set; }
    public ProfessionTitles PlayerProfessions { get; protected set; }
    public ObjectDictionary<MaterialType> Materials { get; protected set; }
	public ObjectDictionary<LeveledItemList> LeveledItems { get; protected set; }

    internal Dictionary<string, Spell> PlayerSpells = new Dictionary<string, Spell>();

    public ObjectManager()
    {

    }

    public void Initialize()
    {
        BigBoss.Debug.w(Logs.Main, "Starting Data Manager");
        NPCs = new NPCDictionary<NPC, NPCInstance>();
        Items = new ItemDictionary();
        Materials = new ObjectDictionary<MaterialType>();
        PlayerProfessions = new ProfessionTitles();
        Strings = new Dictionary<string, string>();
		LeveledItems = new ObjectDictionary<LeveledItemList> ();

        List<UnityEngine.Object> files = new List<UnityEngine.Object>();
        files.AddRange(Resources.LoadAll("XML", typeof(TextAsset)));
        files.AddRange(Resources.LoadAll("XML/NPCs", typeof(TextAsset)));
        files.AddRange(Resources.LoadAll("XML/Strings", typeof(TextAsset)));
        files.AddRange(Resources.LoadAll("XML/Items", typeof(TextAsset)));
        files.AddRange(Resources.LoadAll("XML/LeveledLists", typeof(TextAsset)));

        foreach (UnityEngine.Object file in files)
        {
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.XML))
            {
                BigBoss.Debug.w(Logs.XML, "Parsing " + file.name + ".xml");
            }
            #endregion
            ParseXML((file as TextAsset).text);
        }
        
    }

    #region Parsing
    private void ParseXML(string file)
    {
        XMLNode root = new XMLNode(null); // No parent
        root.Parse(file);
        
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.XML_Print) && BigBoss.Debug.logging(Logs.XML))
        {
            BigBoss.Debug.w(Logs.XML, root.Print());
        }
        #endregion

        ParseXML(root);
    }

    public void ParseXML(XMLNode root)
    {
        string type = root.Key.ToUpper();
        switch (type)
        {
            case "ITEMS":
                Items.Parse(root);
                break;
            case "NPCS":
                NPCs.Parse(root);
                break;
            case "MATERIALS":
                Materials.Parse(root);
                break;
            case "STRINGS":
                ParseStrings(root);
                break;
            case "TITLES":
                PlayerProfessions.ParseXML(root);
                break;
            case "LEVELEDITEMS":
				LeveledItems.Parse (root);
				break;
            default:
                if (BigBoss.Debug.logging(Logs.XML))
                    BigBoss.Debug.w(Logs.XML, "Basenode key " + root.Key + " did not exist as an option to parse.  Node: " + root);
                break;
        }
    }

    void ParseStrings(XMLNode stringsNode)
    {
        foreach (XMLNode stringNode in stringsNode)
        {
            string key = stringNode.SelectString("key");
            if (!Strings.ContainsKey(key))
            {
                Strings.Add(key, stringNode.SelectString("text"));
            }
        }
    }
    #endregion

    // Used by WODictionaries to instantiate their WorldObjects, since they cannot
    public GameObject Instantiate(WorldObject obj, int x, int y)
    {
        return Instantiate(Resources.Load(obj.Prefab), new Vector3(x, 0, y), Quaternion.identity) as GameObject;
    }
}
