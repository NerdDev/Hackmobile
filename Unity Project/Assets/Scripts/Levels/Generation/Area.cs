using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LevelGen
{
    public class Area : LayoutObject<GenSpace>
    {
        public ThemeSet Set;
        public Dictionary<Theme, Theme> PickedPrototypes = new Dictionary<Theme, Theme>(ReferenceEqualityComparer<Theme>.Instance);
        public int NumRooms;
        public int NumRoomsGenerated;
        public Point CenterPt;
        public override Point Center { get { return CenterPt; } }
        private int id;

        public Area(int num)
            : base(LayoutObjectType.Area)
        {
            this.id = num;
        }

        public override string ToString()
        {
            return "Area " + id;
        }
    }
}
