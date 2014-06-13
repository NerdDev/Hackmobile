using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class ThemeMod
{
    public abstract void ModTheme(Theme theme);
}

public class EmptyThemeMod : ThemeMod
{
    public override void ModTheme(Theme theme)
    {
    }
}