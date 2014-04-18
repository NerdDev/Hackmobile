using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class StairElement : ThemeElement
{
    public float StairHeight;
    public bool UpStair;
    public bool DownStair;

    public StairElement()
    {
    }

    public abstract bool Place(Container2D<GenSpace> grid, LayoutObject obj, Theme theme, System.Random rand, out Boxing placed);
}

