using UnityEngine;
using System.Collections;

public class FinalMod : RoomModifier {

    public override void Modify(Room room, System.Random rand)
    {
    }

    public override RoomModType GetType()
    {
        return RoomModType.Final;
    }

    public override string GetName()
    {
        return "Final Mod";
    }
}