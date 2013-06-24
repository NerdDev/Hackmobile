using UnityEngine;
using System.Collections;

public class PillarMod : RoomModifier {

    public override void Modify(Room room)
    {
    }

    public override RoomModType GetType()
    {
        return RoomModType.Flexible;
    }
}
