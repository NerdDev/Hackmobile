using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class StairLink
{
    public Point UpLink { get; set; }
    public Point DownLink { get; set; }
    public Point SelectedLink { get; set; }

    public void Select(bool up)
    {
        SelectedLink = up ? UpLink : DownLink;
    }

    public StairLink(Point p, bool selectUp)
    {
        UpLink = new Point(p);
        DownLink = new Point(p);
        Select(selectUp);
    }
}