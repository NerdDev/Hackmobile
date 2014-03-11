using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GenDeploy : ITransform, IEnumerable<Value2D<GenSpace>>
{
    public ThemeElement Element;
    public MultiMap<GenSpace> Spaces;
    public float Rotation { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public bool Deployed = false;
    public Point OriginPt;
    
    public GenDeploy(ThemeElement element)
    {
        Element = element;
        Spaces = new MultiMap<GenSpace>();
    }

    public void AddSpace(GenSpace space, int x, int y)
    {
        space.AddDeploy(this, x, y);
    }

    public IEnumerator<Value2D<GenSpace>> GetEnumerator()
    {
        foreach (Value2D<GenSpace> v in Spaces)
        {
            v.x -= OriginPt.x;
            v.y -= OriginPt.y;
            yield return v;
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}

