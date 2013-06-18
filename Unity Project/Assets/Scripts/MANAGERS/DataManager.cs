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
    Dictionary<string, Item> baseItems = new Dictionary<string, Item>();
    Dictionary<string, NPC> baseNPCs = new Dictionary<string, NPC>();
    Dictionary<string, MaterialType> materials = new Dictionary<string, MaterialType>();
    Dictionary<string, Dice> Dice = new Dictionary<string, Dice>();
    #endregion

    #region XML Paths.
    //TODO: Abstract all XML paths to a central XML which links to the rest of the XML's?
    string npcsPath = "Assets/Resources/XML/npcs.xml";
    string itemsPath = "Assets/Resources/XML/items.xml";
    string materialsPath = "Assets/Resources/XML/materials.xml";
    //Optional TODO: Add file path searching for additional XML's to load and parse by headers.
    #endregion

    #region Null Data for Errors
    MaterialType nullMaterial { get; set; }
    Item nullItem { get; set; }

    private void initializeNullData()
    {
        //Null material
        nullMaterial = new MaterialType();
        nullMaterial.setNull();

        //Null item
        GameObject nullGOItem = new GameObject("nullItem");
        nullItem = nullGOItem.AddComponent<Item>();
        nullItem.setNull();
    }
    #endregion

    void Start ()
    {
        initializeNullData();

        //Order matters here.
        xmlMaterials = new XMLReader(materialsPath);
        parseMaterials(xmlMaterials.getRoot());

        xmlItems = new XMLReader(itemsPath);
        parseItems(xmlItems.getRoot());

        xmlNPCs = new XMLReader(npcsPath);

    }

    public Item getItem(string itemName)
    {
        if (getItems().ContainsKey(itemName))
        {
            return BigBoss.DataManager.getItems()[itemName];
        }
        else
        {
            return nullItem;
        }
    }

    public MaterialType getMaterial(string mat)
    {
        if (getMaterials().ContainsKey(mat))
        {
            return getMaterials()[mat];
        }
        else
        {
            return nullMaterial;
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
    void parseItems(XMLNode x)
    {
        foreach (XMLNode m in x.select("weapons").get())
        {
            parseItem(m, "weapon");
        }
    }

    private void parseItem(XMLNode m, string type)
    {
        string itemName = m.select("name").getText();
        GameObject go = new GameObject(itemName);
        Item i = go.AddComponent<Item>();
        i.Type = type;
        i.Name = itemName;
        i.Damage = this.getDice(m.select("damage").getText());
        i.Material = getMaterial(m.select("material").getText());
        i.Model = m.select("model").getText();
        i.ModelTexture = m.select("modeltexture").getText();
        //If parsing fails, returns to 0.
        int tempWeight, tempCost;
        int.TryParse(m.select("weight").getText(), out tempWeight);
        i.Weight = tempWeight;
        int.TryParse(m.select("cost").getText(), out tempCost);
        i.Cost = tempCost;

        baseItems.Add(i.Name, i);
    }

    void parseMaterials(XMLNode x)
    {
        foreach (XMLNode m in x.select("materials").get())
        {
            MaterialType mat = new MaterialType();
            mat.Name = m.select("name").getText();
            int tempHardness;
            int.TryParse(m.select("hardness").getText(), out tempHardness);
            mat.Hardness = tempHardness;
            bool tempBurns, tempOxidizes;
            bool.TryParse(m.select("burns").getText(), out tempBurns);
            mat.Burns = tempBurns;
            bool.TryParse(m.select("oxidizes").getText(), out tempOxidizes);
            mat.Oxidizes = tempOxidizes;

            materials.Add(mat.Name, mat);
        }
    }

    void parseNPCs(XMLNode x)
    {
        foreach (XMLNode m in x.select("npcs").get())
        {
            
            string npcName = m.select("name").getText();
            GameObject go = new GameObject(npcName);
            NPC n = go.AddComponent<NPC>();
            n.Name = npcName;
            n.role = (NPCRole) Enum.Parse(typeof(NPCRole), m.select("role").getText(), true);
            n.race = (NPCRace) Enum.Parse(typeof(NPCRace), m.select("race").getText(), true);
            n.Model = m.select("model").getText();
            n.ModelTexture = m.select("modeltexture").getText();

            //property parsing
            string temp = m.select("properties").getText();
            string[] split = temp.Split(',');
            for (int i = 0; i < split.Length; i++) 
            {
                n.props = n.props.Include<NPCProperties>((NPCProperties) Enum.Parse(typeof(NPCProperties), split[i], true));
            }

            //flag parsing
            temp = m.select("flags").getText();
            split = temp.Split(',');
            for (int i = 0; i < split.Length; i++) 
            {
                n.flags = n.flags.Include<NPCFlags>((NPCFlags) Enum.Parse(typeof(NPCFlags), split[i], true));
            }

            //body part data
            n.bodyparts.parseXML(m.select("bodyparts"));

            //write npc stat parser
            n.stats.parseXML(m.select("stats"));

            //write inventory parser
            //inventory

            baseNPCs.Add(n.Name, n);
            
        }
    }
    #endregion

    #region Map returns (should be abstracted to other methods for most purposes).
    Dictionary<string, Item> getItems()
    {
        return baseItems;
    }

    Dictionary<string, NPC> getNPCs()
    {
        return baseNPCs;
    }

    Dictionary<string, MaterialType> getMaterials()
    {
        return materials;
    }

    Dictionary<string, Dice> getDice()
    {
        return Dice;
    }
    #endregion
}
