using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LevelGen
{
    public class Area : LayoutObject<GenSpace>
    {
        public ThemeSet Set;
        public int NumRooms;
        public int NumRoomsGenerated;
        public Point Center;
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
