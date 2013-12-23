using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StairLinks : IEnumerable<Point>
{
    private bool _upSet = false;
    private bool _downSet = false;
    public bool Set {
        get
        {
            return SelectedUp ? _upSet : _downSet;
        }
        set
        {
            if (SelectedUp)
                _upSet = value;
            else
                _downSet = value;
        }
    }
    public bool SelectedUp { get; set; }
    List<StairLink> links = new List<StairLink>();

    public void Center()
    {
        Bounding bounds = new Bounding();
        foreach (StairLink link in links)
            bounds.Absorb(link.Get(SelectedUp));
        Point shift = new Point();
        shift = shift - bounds.GetCenter();
        foreach (StairLink link in links)
            link.Get(SelectedUp).Shift(shift);
    }

    public void Add(Point p)
    {
        links.Add(new StairLink(p));
    }

    public void Shift(Point p)
    {
        foreach (StairLink link in links)
            link.Get(SelectedUp).Shift(p);
    }

    public bool Intersect(StairLinks rhs)
    {
        HashSet<Point> set = new HashSet<Point>();
        foreach (StairLink link in links)
            set.Add(link.Get(SelectedUp));
        foreach (StairLink link in rhs.links)
        {
            if (set.Contains(link.Get(SelectedUp))) return true;
        }
        return false;
    }

    protected class StairLink
    {
        public Point UpLink { get; set; }
        public Point DownLink { get; set; }
        public Point Get(bool up)
        {
            return up ? UpLink : DownLink;
        }
        public StairLink(Point p)
        {
            UpLink = new Point(p);
            DownLink = new Point(p);
        }
    }

    public IEnumerator<Point> GetEnumerator()
    {
        foreach (StairLink link in links)
        {
            yield return link.Get(SelectedUp);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
