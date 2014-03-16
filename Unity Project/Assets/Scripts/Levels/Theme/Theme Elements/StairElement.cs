using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class StairElement : ThemeElement
{
    public StairElement()
    {
        SetChar('/');
    }

    public abstract bool Place(LayoutObject obj, Theme theme, System.Random rand, out Bounding placed);
}

