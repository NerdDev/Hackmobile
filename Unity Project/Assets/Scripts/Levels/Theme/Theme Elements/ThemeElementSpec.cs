using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ThemeElementSpec : IEnumerable<Value2D<GenSpace>>
{
    public GridType Type;
    public Theme Theme;
    public GenDeploy GenDeploy;
    public GenSpace GenSpace;
    public GridSpace Space;
    public Container2D<GenSpace> GenGrid;
    public Container2D<GridSpace> Grid;
    public MultiMap<GenSpace> DeployGrid = new MultiMap<GenSpace>();
    public System.Random Random;
    public int DeployX;
    public int DeployY;
    public MultiMap<List<GenDeploy>> Additional = new MultiMap<List<GenDeploy>>();
    private Bounding _bounding;
    public Bounding Bounding
    {
        get
        {
            if (_bounding == null)
            {
                _bounding = new Bounding();
                foreach (var v in this)
                {
                    _bounding.Absorb(v);
                }
            }
            return _bounding;
        }
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

    public void Reset()
    {
        _bounding = null;
        Additional.Clear();
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
