using UnityEngine;
using System;
using System.Collections.Generic;
using XML;
using System.IO;


public class DataManager : MonoBehaviour, IManager
{
    string XMLPath = "Assets/Scripts/XML/";
    Dictionary<string, Action<XMLNode>> parsing = new Dictionary<string, Action<XMLNode>>();
    public Dictionary<string, string> strings = new Dictionary<string, string>();
    public ProfessionTitles playerProfessions = new ProfessionTitles();
    public Dictionary<string, MaterialType> Materials = new Dictionary<string, MaterialType>();

    public void Initialize()
    {
        BigBoss.Debug.w(Logs.Main, "Starting Data Manager");
        //Parsing functions here
        parsing.Add("items", ParseItems);
        parsing.Add("npcs", parseNPCs);
        parsing.Add("materials", parseMaterials);
        parsing.Add("strings", parseStrings);
        parsing.Add("titles", parseTitles);

        string[] files = Directory.GetFiles(XMLPath, "*.xml", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            BuildXML(file);
        }
    }

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

    #region XML Parsing methods.

    void ParseXML(XMLNode root)
    {
        if (parsing.ContainsKey(root.Key))
        {
            parsing[root.Key](root);
        }
        else if (BigBoss.Debug.logging(Logs.XML))
            BigBoss.Debug.w(Logs.XML, "Basenode key " + root.Key + " did not exist as an option to parse.  Node: " + root);
    }

    void ParseItems(XMLNode itemsNode)
    {
        foreach (XMLNode categoryNode in itemsNode)
        {
            ItemDictionary items = BigBoss.WorldObject.Items;
            foreach (XMLNode itemNode in categoryNode)
            {
                Item i = parseItem(itemNode);
                if (!items.Add(i, categoryNode.Key) && BigBoss.Debug.logging(Logs.XML))
                {
                    BigBoss.Debug.w(Logs.XML, "Item already existed with name: " + i.Name + " under node " + itemNode);
                }
            }
        }
    }

    private Item parseItem(XMLNode itemNode)
    {
        string itemName = itemNode.SelectString("name");
        Item i = new Item();
        i.Type = itemNode.Key;
        i.Name = itemName;
        i.ParseXML(itemNode);
        return i;
    }

    void parseMaterials(XMLNode materialsNode)
    {
        foreach (XMLNode materialNode in materialsNode)
        {
            MaterialType mat = new MaterialType();
            mat.parseXML(materialNode);
            Materials.Add(mat.Name, mat);
        }
    }

    void parseNPCs(XMLNode npcsNode)
    {
        WODictionary<NPC> npcs = BigBoss.WorldObject.NPCs;
        foreach (XMLNode npcNode in npcsNode)
        {
            NPC n = new NPC();
            n.ParseXML(npcNode);
            if (!npcs.Add(n) && BigBoss.Debug.logging(Logs.XML))
            {
                BigBoss.Debug.w(Logs.XML, "NPC already existed with name: " + n.Name + " under node " + npcNode);
            }
        }
    }

    void parseStrings(XMLNode stringsNode)
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

    void parseTitles(XMLNode x)
    {
        playerProfessions.parseXML(x);
    }
    #endregion
}
