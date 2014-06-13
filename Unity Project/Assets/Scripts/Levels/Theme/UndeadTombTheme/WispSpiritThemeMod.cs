using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class WispSpiritThemeMod : ThemeMod
{
    public override void ModTheme(Theme theme)
    {
        theme.SpawnMods.AreaMods.Add(new SpawnNPCs()
        {
            CustomKeywords = new SpawnKeywords[] 
            { 
                SpawnKeywords.TOMB_WISP
            }
        });
    }
}

