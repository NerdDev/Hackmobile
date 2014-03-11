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
    public int DeployX;
    public int DeployY;
    public MultiMap<List<GenDeploy>> Additional = new MultiMap<List<GenDeploy>>();

    public Bounding GetBounds()
    {
        Bounding bounds = new Bounding();
        foreach (var v in this)
        {
            bounds.Absorb(v);
        }
        return bounds;
    }

    public void AddAdditional(GenDeploy deploy, int x, int y)
    {
        List<GenDeploy> list;
        if (!Additional.TryGetValue(x, y, out list))
        {
            list = new List<GenDeploy>(1);
            Additional[x, y] = list;
        }
        list.Add(deploy);
    }

    public IEnumerator<Value2D<GenSpace>> GetEnumerator()
    {
        foreach (var space in GenDeploy)
        {
            space.x += DeployX;
            space.y += DeployY;
            yield return space;
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
