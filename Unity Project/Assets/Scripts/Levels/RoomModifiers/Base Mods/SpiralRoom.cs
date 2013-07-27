using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class SpiralRoom : RoomModifier {
    public override void Modify(Room room, System.Random rand)
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, ToString());
        }
        #endregion
        int side = LevelGenerator.Rand.Next(LevelGenerator.minRectSize + 5, LevelGenerator.maxRectSize);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.w(DebugManager.Logs.LevelGen, "Height: " + side + ", Width: " + side);
        }
        #endregion
        room.BoxStrokeAndFill(GridType.Wall, GridType.Floor, side, side);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
        int row = 0;
        int col = 1;
        int mod = 2;
        bool complete = false;
        while (!complete)
        {
            while (row < side - mod - 1)
            {
                row++;
                room.put(GridType.Wall, row, col);
            }
            if (side - mod - 1 < side / 2 || mod > side / 2)
            {
                complete = true;
                break;
            }
            while (col < side - mod - 1)
            {
                col++;
                room.put(GridType.Wall, row, col);
            }
            if (side - mod - 1 < side / 2 || mod > side / 2)
            {
                complete = true;
                break;
            }
            while (col > mod)
            {
                col--;
                room.put(GridType.Wall, row, col);
            }
            if (side - mod - 1 < side / 2 || mod > side / 2)
            {
                complete = true;
                break;
            }
            mod += 2;
            while (row > mod)
            {
                row--;
                room.put(GridType.Wall, row, col);
            }
            if (side - mod - 1 < side / 2 || mod > side / 2)
            {
                complete = true;
                break;
            }
        }
    }

    public override RoomModType GetType()
    {
        return RoomModType.Base;
    }

    public override string GetName()
    {
        return "Spiral Room";
    }
}
