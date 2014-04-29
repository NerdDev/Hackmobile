using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class NPCMemoryItem
{
    public bool CanSee;
    public bool AwareOf;
    public ulong TurnLastSeen;
    public GridSpace SpaceLastSeen;
    public bool Friendly;
    public NPC NPC;

    public NPCMemoryItem(NPC npc)
    {
        this.NPC = npc;
    }
}
