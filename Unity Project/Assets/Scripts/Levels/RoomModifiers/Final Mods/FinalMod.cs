using UnityEngine;
using System.Collections;

public class FinalMod : RoomModifier {

    public override bool Modify(RoomSpec spec)
    {
        return true;
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
