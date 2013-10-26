using UnityEngine;
using System;
using System.Collections.Generic;
using XML;
using System.IO;


public class DataManager : MonoBehaviour, IManager
{
    #region XML Paths.
    string XMLPath = "Assets/Scripts/XML/";

    Dictionary<string, Action<XMLNode>> parsing = new Dictionary<string, Action<XMLNode>>();
    #endregion

    #region Strings
    public Dictionary<string, string> strings = new Dictionary<string, string>();
    #endregion

    #region Titles
    public ProfessionTitles playerProfessions = new ProfessionTitles();
    #endregion

    public void Initialize()
    {
        BigBoss.Debug.w(Logs.Main, "Starting Data Manager");
        //Parsing functions here
        parsing.Add("items", parseItems);
        parsing.Add("npcs", parseNPCs);
        parsing.Add("materials", parseMaterials);
        parsing.Add("strings", parseStrings);
        parsing.Add("titles", parseTitles);

        string[] files = Directory.GetFiles(XMLPath, "*.xml", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            buildXML(file);
        }
    }

    void Start()
    {
    }

    private void buildXML(string file)
    {
        if (BigBoss.Debug.logging(Logs.XML))
            BigBoss.Debug.w(Logs.XML, "Parsing " + file);

        XMLNode root = new XMLNode(null); // No parent
        root.Parse(System.IO.File.ReadAllText(file));

        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.XML_Print) && BigBoss.Debug.logging(Logs.XML))
            BigBoss.Debug.w(Logs.XML, root.Print());

        parseXML(root);
    }

    #region XML Parsing methods.

    void parseXML(XMLNode root)
    {
        if (parsing.ContainsKey(root.Key))
        {
            parsing[root.Key](root);
        }
        else if (BigBoss.Debug.logging(Logs.XML))
            BigBoss.Debug.w(Logs.XML, "Basenode key " + root.Key + " did not exist as an option to parse.  Node: " + root);
    }

    void parseItems(XMLNode itemsNode)
    {
        foreach (XMLNode categoryNode in itemsNode)
        {
            List<Item> addedItems = new List<Item>();
            Dictionary<string, Item> items = BigBoss.WorldObject.getItems();
            foreach (XMLNode itemNode in categoryNode)
            {
                Item i = parseItem(itemNode);
                if (!items.ContainsKey(i.Name))
                {
                    addedItems.Add(i);
                    BigBoss.WorldObject.getItems().Add(i.Name, i);
                }
                else if (BigBoss.Debug.logging(Logs.XML))
                    BigBoss.Debug.w(Logs.XML, "Item already existed with name: " + i.Name + " under node " + itemNode);
            }
            BigBoss.WorldObject.getCategories().Add(categoryNode.Key, addedItems);
        }
    }

    private Item parseItem(XMLNode itemNode)
    {
        string itemName = itemNode.SelectString("name");
        GameObject go = new GameObject(itemName);
        Item i = go.AddComponent<Item>();
        i.Type = itemNode.Key;
        i.Name = itemName;
        i.parseXML(itemNode);
        return i;
    }

    void parseMaterials(XMLNode materialsNode)
    {
        foreach (XMLNode materialNode in materialsNode)
        {
            MaterialType mat = new MaterialType();
            mat.parseXML(materialNode);
            BigBoss.WorldObject.getMaterials().Add(mat.Name, mat);
        }
    }

    void parseNPCs(XMLNode npcsNode)
    {
        Dictionary<string, NPC> npcs = BigBoss.WorldObject.getNPCs();
        foreach (XMLNode npcNode in npcsNode)
        {
            string npcName = npcNode.Name;
            GameObject go = new GameObject(npcName);
            NPC n = go.AddComponent<NPC>();
            n.Name = npcName;
            n.parseXML(npcNode);
            if (!npcs.ContainsKey(n.Name))
            {
                npcs.Add(n.Name, n);
            }
            else if (BigBoss.Debug.logging(Logs.XML))
                BigBoss.Debug.w(Logs.XML, "NPC already existed with name: " + n.Name + " under node " + npcNode);
            n.IsActive = false;
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
