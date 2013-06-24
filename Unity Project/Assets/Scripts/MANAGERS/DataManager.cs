using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XML;

public class DataManager : MonoBehaviour
{
    //TODO: Write deletion code for when these are parsed and stored.
    XMLReader xmlItems;
    XMLReader xmlNPCs;
    XMLReader xmlMaterials;

    #region Storage Maps.
    Dictionary<string, Dice> Dice = new Dictionary<string, Dice>();
    #endregion

    #region XML Paths.
    //TODO: Abstract all XML paths to a central XML which links to the rest of the XML's?
    string npcsPath = "Assets/Resources/XML/npcs.xml";
    string itemsPath = "Assets/Resources/XML/items.xml";
    string materialsPath = "Assets/Resources/XML/materials.xml";
    //Optional TODO: Add file path searching for additional XML's to load and parse by headers.
    #endregion

    void Start ()
    {
        //Order matters here.
        xmlMaterials = new XMLReader(materialsPath);
        parseMaterials(xmlMaterials.getRoot());

        xmlItems = new XMLReader(itemsPath);
        parseItems(xmlItems.getRoot());

        xmlNPCs = new XMLReader(npcsPath);
        parseNPCs(xmlNPCs.getRoot());
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
    void parseItems(XMLNode x)
    {
        foreach (XMLNode m in x.select("weapons").get())
        {
            parseItem(m, "weapon");
        }
    }

    private void parseItem(XMLNode m, string type)
    {
        string itemName = m.SelectString("name");
        GameObject go = new GameObject(itemName);
        Item i = go.AddComponent<Item>();
        i.Type = type;
        i.Name = itemName;
        i.parseXML(m);
        BigBoss.ItemMaster.getItems().Add(i.Name, i);
    }

    void parseMaterials(XMLNode x)
    {
        foreach (XMLNode m in x.select("materials").get())
        {
            MaterialType mat = new MaterialType();
            mat.parseXML(m);
            BigBoss.ItemMaster.getMaterials().Add(mat.Name, mat);
        }
    }

    void parseNPCs(XMLNode x)
    {
        foreach (XMLNode m in x.select("npcs").get())
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
