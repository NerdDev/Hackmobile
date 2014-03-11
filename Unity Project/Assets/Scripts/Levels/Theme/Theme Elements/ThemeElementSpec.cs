using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ThemeElementSpec
{
    public GridType Type;
    public Theme Theme;
    public GenDeploy GenDeploy;
    public GridSpace Space;
    public Container2D<GenSpace> GenGrid;
    public Container2D<GridSpace> Grid;
    public System.Random Random;
    public int X;
    public int Y;
}
