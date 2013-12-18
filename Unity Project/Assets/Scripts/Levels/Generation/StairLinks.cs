using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StairLinks {

    List<StairLink> links = new List<StairLink>();

    public void Center()
    {
        Bounding bounds = new Bounding();
        foreach (StairLink link in links)
            bounds.Absorb(link.SelectedLink);
        Point shift = new Point();
        shift = shift - bounds.GetCenter();
        foreach (StairLink link in links)
            link.SelectedLink.Shift(shift);
    }

    public void Select(bool up)
    {
        foreach (StairLink link in links)
            link.SelectedLink = up ? link.UpLink : link.DownLink;
    }

    protected class StairLink
    {
        public Point UpLink { get; set; }
        public Point DownLink { get; set; }
        public Point SelectedLink { get; set; }
    }
}
