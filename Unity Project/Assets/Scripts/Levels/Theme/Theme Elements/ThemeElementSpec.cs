using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ThemeElementSpec : IEnumerable<Value2D<GenSpace>>
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

    public Bounding GetBounds()
    {
        Bounding bounds = new Bounding();
        foreach (var v in this)
        {
            bounds.Absorb(v);
        }
        return bounds;
    }

    public IEnumerator<Value2D<GenSpace>> GetEnumerator()
    {
        foreach (var space in GenDeploy)
        {
            space.x += X;
            space.y += Y;
            yield return space;
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
