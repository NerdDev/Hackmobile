using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XML;
using System.IO;

public class DataManager : MonoBehaviour
{
    #region XML Paths.
    string XMLPath = "Assets/Resources/XML/";

    Dictionary<string, Action<XMLNode>> parsing = new Dictionary<string, Action<XMLNode>>();
    #endregion

    #region Strings
    public Dictionary<string, string> strings = new Dictionary<string, string>();
    #endregion

    void Start ()
    {
        //Parsing functions here
        parsing.Add("items", parseItems);
        parsing.Add("npcs", parseNPCs);
        parsing.Add("materials", parseMaterials);
        parsing.Add("strings", parseStrings);

        string[] files = Directory.GetFiles(XMLPath, "*.xml", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            XMLReader xreader = new XMLReader(file);
            parseXML(xreader);
            xreader = null; //lets GC collect up.
        }

        BigBoss.NPCManager.Log();
    }

    #region XML Parsing methods.

    void parseXML(XMLReader xreader)
    {
        foreach (XMLNode x in xreader.getRoot().get()) {
            parsing[x.getKey()](x);
        }
    }
    
    void parseItems(XMLNode top)
    {
        foreach (XMLNode x in top.get())
        {
            List<Item> items = new List<Item>();
            foreach (XMLNode xnode in x.get())
            {
                items.Add(parseItem(xnode));
            }
            BigBoss.ItemMaster.getCategories().Add(x.getKey(), items);
        }
    }

    private Item parseItem(XMLNode x)
    {
        string itemName = x.SelectString("name");
        GameObject go = new GameObject(itemName);
        Item i = go.AddComponent<Item>();
        i.Type = x.getKey();
        i.Name = itemName;
        i.parseXML(x);
        return i;
    }

    void parseMaterials(XMLNode x)
    {
        foreach (XMLNode m in x.get())
        {
            MaterialType mat = new MaterialType();
            mat.parseXML(m);
            BigBoss.ItemMaster.getMaterials().Add(mat.Name, mat);
        }
    }

    void parseNPCs(XMLNode x)
    {
        foreach (XMLNode m in x.get())
        {
            string npcName = m.SelectString("name");
            GameObject go = new GameObject(npcName);
            NPC n = go.AddComponent<NPC>();
            n.Name = npcName;
            n.parseXML(m);
            BigBoss.NPCManager.getNPCs().Add(n.Name, n);
        }
    }

    void parseStrings(XMLNode x)
    {
        foreach (XMLNode m in x.get())
        {
            string key = m.SelectString("key");
            if (!strings.ContainsKey(key))
            {
                strings.Add(key, m.SelectString("text"));
            }
        }
    }
    #endregion
}
