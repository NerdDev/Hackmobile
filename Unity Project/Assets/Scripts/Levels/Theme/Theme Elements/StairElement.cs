using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class StairElement : ThemeElement
{
    public virtual DrawAction<GenSpace> UnitTest
    {
        get
        {
            return Draw.IsType<GenSpace>(GridType.Floor).
                // If not blocking a path
                And(Draw.Not(Draw.Blocking(Draw.Walkable())));
        }
    }
    public virtual DrawAction<GenSpace> FrontTest { get { return Draw.IsType<GenSpace>(GridType.Floor); } }
    public virtual DrawAction<GenSpace> BackTest { get { return Draw.True<GenSpace>(); } }
    public float StairHeight;
    public bool UpStair;
    public bool DownStair;

    public StairElement()
    {
    }

    public abstract bool Place(Container2D<GenSpace> grid, LayoutObject obj, Theme theme, System.Random rand, out Boxing placed);
}

