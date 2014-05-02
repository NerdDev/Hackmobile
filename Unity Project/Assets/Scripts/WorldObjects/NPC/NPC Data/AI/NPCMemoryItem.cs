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

    public void ToLog(Log log)
    {
        if (!BigBoss.Debug.Flag(DebugManager.DebugFlag.AI)) return;
        log.printHeader(NPC + " Memory Item");
        log.w("Can See: " + CanSee);
        log.w("Aware of: " + AwareOf);
        log.w("Turn Last Seen: " + TurnLastSeen);
        log.w("Space Last Seen: " + SpaceLastSeen);
        log.w("Friendly: " + Friendly);
        log.printFooter(NPC + " Memory Item");
    }
}
