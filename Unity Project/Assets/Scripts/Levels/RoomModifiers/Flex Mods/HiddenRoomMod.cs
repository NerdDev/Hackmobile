using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HiddenRoomMod : RoomModifier
{
    public override bool Modify(RoomSpec spec)
    {
        int secretRoomSize = 2;
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Hidden Room Mod");
        }
        #endregion

        RandomPicker<GridType> picker;
        spec.Array.DrawPotentialExternalDoors(Draw.PickRandom(out picker));
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            picker.ToLog(Logs.LevelGen, spec.Array, "After Removing Invalid Locations");
        }
        #endregion

        Value2D<GridType> doorSpace = picker.Pick(spec.Random);
        if (doorSpace != null)
        {
            spec.Array.DrawSquare(
                (doorSpace.x - secretRoomSize), (doorSpace.x + secretRoomSize),
                (doorSpace.y - secretRoomSize), (doorSpace.y + secretRoomSize),
                new StrokedAction<GridType>()
                {
                    UnitAction = Draw.SetTo(GridType.NULL, GridType.Floor),
                    StrokeAction = Draw.SetTo(GridType.NULL, GridType.Wall)
                });
            spec.Array[doorSpace.x, doorSpace.y] = GridType.Door;
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            spec.Room.ToLog(Logs.LevelGen, "Final Room After placing doors");
            BigBoss.Debug.printFooter(Logs.LevelGen, "Hidden Room Mod");
        }
        #endregion
        return true;
    }

    public override RoomModType GetType()
    {
        return RoomModType.Flexible;
    }

    public override string GetName()
    {
        return "Hidden Room";
    }
}
