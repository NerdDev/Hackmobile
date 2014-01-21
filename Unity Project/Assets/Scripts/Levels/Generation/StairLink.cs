using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class StairLink
{
    public Point UpLink;
    public Point DownLink;
    public bool SelectedUp { get; set; }
    public Point SelectedLink { get { return SelectedUp ? UpLink : DownLink; } }

    public StairLink(Point p, bool selectUp)
    {
        UpLink = new Point(p);
        DownLink = new Point(p);
        SelectedUp = selectUp;
    }
}