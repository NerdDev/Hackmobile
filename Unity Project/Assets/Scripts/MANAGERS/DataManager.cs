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

    void Start ()
    {
    }

    private void buildXML(string file)
    {
        XMLNode root = new XMLNode(null); // No parent
        root.Parse(System.IO.File.ReadAllText(file));
        parseXML(root);
    }

    #region XML Parsing methods.

    void parseXML(XMLNode root)
    {
        foreach (XMLNode baseNode in root)
        {
            parsing[baseNode.Key](baseNode);
        }
    }
    
    void parseItems(XMLNode itemsNode)
    {
        foreach (XMLNode categoryNode in itemsNode)
        {
            List<Item> items = new List<Item>();
            foreach (XMLNode itemNode in categoryNode)
            {
                Item i = parseItem(itemNode);
                items.Add(i);
                BigBoss.WorldObject.getItems().Add(i.Name, i);
            }
            BigBoss.WorldObject.getCategories().Add(categoryNode.Key, items);
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
        foreach (XMLNode npcNode in npcsNode)
        {
            string npcName = npcNode.Name;
            GameObject go = new GameObject(npcName);
            NPC n = go.AddComponent<NPC>();
            n.Name = npcName;
            n.parseXML(npcNode);
            BigBoss.WorldObject.getNPCs().Add(n.Name, n);
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
