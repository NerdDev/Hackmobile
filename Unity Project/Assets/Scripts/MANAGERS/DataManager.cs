using UnityEngine;
using System;
using System.Collections.Generic;
using XML;
using System.IO;


public class DataManager : MonoBehaviour, IManager
{
    const string XMLPath = "Assets/Scripts/XML/";
    public WODictionary<NPC> NPCs { get; protected set; }
    public ItemDictionary Items { get; protected set; }
    public Dictionary<string, string> strings = new Dictionary<string, string>();
    public ProfessionTitles PlayerProfessions = new ProfessionTitles();
    public ObjectDictionary<MaterialType> Materials { get; protected set; }

    public DataManager()
    {
    }

    public void Initialize()
    {
        BigBoss.Debug.w(Logs.Main, "Starting Data Manager");
        NPCs = new WODictionary<NPC>();
        Items = new ItemDictionary();
        Materials = new ObjectDictionary<MaterialType>();

        string[] files = Directory.GetFiles(XMLPath, "*.xml", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            BuildXML(file);
        }
    }

    #region XML
    private void BuildXML(string file)
    {
        if (BigBoss.Debug.logging(Logs.XML))
            BigBoss.Debug.w(Logs.XML, "Parsing " + file);

        XMLNode root = new XMLNode(null); // No parent
        root.Parse(System.IO.File.ReadAllText(file));

        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.XML_Print) && BigBoss.Debug.logging(Logs.XML))
            BigBoss.Debug.w(Logs.XML, root.Print());

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
            if (!strings.ContainsKey(key))
            {
                strings.Add(key, stringNode.SelectString("text"));
            }
        }
    }
    #endregion

    // Used by WODictionaries to instantiate their WorldObjects, since they cannot
    public GameObject Instantiate(WorldObject obj, int x, int y)
    {
        return Instantiate(Resources.Load(obj.Prefab), new Vector3(x, -.5f, y), Quaternion.identity) as GameObject;
    }
}
