using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LevelGen
{
    public class Area : LayoutObjectContainer<GenSpace>
    {
        public ThemeSet Set;
        public int NumRooms;
        public int NumRoomsGenerated;
        public Point Center;
    }
}
