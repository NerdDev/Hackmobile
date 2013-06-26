using UnityEngine;
using System.Collections;

public class SquareRoom : RoomModifier {

    public override void Modify(Room room, System.Random rand)
    {
        // Nothing
    }

    public override RoomModType GetType()
    {
        return RoomModType.Base;
    }

    public override string GetName()
    {
        return "Square Room";
    }
}
