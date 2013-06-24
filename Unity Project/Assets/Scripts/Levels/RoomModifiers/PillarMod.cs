using UnityEngine;
using System.Collections;

public class PillarMod : RoomModifier {

    public override void Modify(Room room, System.Random rand)
    {
    }

    public override RoomModType GetType()
    {
        return RoomModType.Flexible;
    }
}
