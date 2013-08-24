using System;
using System.Collections.Generic;
using XML;

public class ProfessionTitles
{
    Titles[] titles = new Titles[(int) PlayerProfessions.LAST];
    public bool isParsed = false;

    public void parseXML(XMLNode x)
    {
        if (x != null)
        {
            foreach (XMLNode xnode in x.get())
            {
                PlayerProfessions prof = xnode.SelectEnum<PlayerProfessions>("name");
                titles[(int)prof] = new Titles();
                Titles t = titles[(int)prof];
                t.parseXML(xnode);
            }
        }
    }

    public string getTitle(PlayerProfessions prof, int level)
    {
        return titles[(int)prof].getTitle(level);
    }

    internal class Titles
    {
        public List<Title> titles = new List<Title>();

        public void parseXML(XMLNode x)
        {
            foreach (XMLNode xmlNode in x.selectList("title"))
            {
                titles.Add(new Title(xmlNode.SelectInt("level"), xmlNode.SelectString("playertitle")));
            }
            titles.Reverse();
        }
        
        public string getTitle(int level) {
            foreach (Title t in titles)
            {
                if (t.levelReq < level)
                {
                    return t.titleName;
                }
            }
            return titles[0].titleName;
        }

        internal class Title
        {
            public int levelReq;
            public string titleName;

            public Title(int lr, string tn)
            {
                this.levelReq = lr;
                this.titleName = tn;
            }
        }
    }
}