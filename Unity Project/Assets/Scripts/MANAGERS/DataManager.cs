using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XML;
using System.IO;

public class DataManager : MonoBehaviour
{
    #region Storage Maps.
    Dictionary<string, Dice> Dice = new Dictionary<string, Dice>();
    #endregion

    #region XML Paths.
    string XMLPath = "Assets/Resources/XML/";

    Dictionary<string, Action<XMLNode>> parsing = new Dictionary<string, Action<XMLNode>>();
    #endregion

    void Start ()
    {
        //Parsing functions here
        parsing.Add("items", parseItems);
        parsing.Add("npcs", parseNPCs);
        parsing.Add("materials", parseMaterials);

        string[] files = Directory.GetFiles(XMLPath, "*.xml", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            Debug.Log(file);
            parseXML(new XMLReader(file));
        }
    }

    public Dice getDice(string dice)
    {
        if (Dice.ContainsKey(dice))
        {
            return Dice[dice];
        }
        else
        {
            Dice d = new Dice(dice);
            Dice.Add(dice, d);
            return d;
        }
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
    #endregion

    #region Map returns (should be abstracted to other methods for most purposes).
    Dictionary<string, Dice> getDice()
    {
        return Dice;
    }
    #endregion
}
