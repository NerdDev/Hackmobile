using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HiddenRoomMod : FillRoomMod
{
    public override RoomModType ModType { get { return RoomModType.Flexible; } }
    public override string Name { get { return "Hidden Room"; } }

    public override bool Modify(RoomSpec spec)
    {
        int secretRoomSize = 2;
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Hidden Room Mod");
        }
        #endregion

        RandomPicker<GenSpace> picker;
        spec.Grids.DrawPotentialExternalDoors(Draw.PickRandom(out picker));

        Value2D<GenSpace> doorSpace = picker.Pick(spec.Random);
        if (doorSpace != null)
        {
            spec.Grids.DrawSquare(
                (doorSpace.x - secretRoomSize), (doorSpace.x + secretRoomSize),
                (doorSpace.y - secretRoomSize), (doorSpace.y + secretRoomSize),
                new StrokedAction<GenSpace>()
                {
                    UnitAction = Draw.SetTo(GridType.NULL, new GenSpace(GridType.Floor, spec.Theme)),
                    StrokeAction = Draw.SetTo(GridType.NULL, new GenSpace(GridType.Wall, spec.Theme))
                });
            spec.Grids.SetTo(doorSpace.x, doorSpace.y, new GenSpace(GridType.Door, spec.Theme));
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
}
