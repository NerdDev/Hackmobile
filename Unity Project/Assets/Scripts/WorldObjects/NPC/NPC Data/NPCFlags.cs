using System;

[FlagsAttribute]
public enum NPCFlags
{
    NONE,
    NO_CORPSE, // Doesn't leave a corpse on death
    POLY, //player can turn into one of these
    NO_RANDOM_SPAWN, // Won't be picked if a random NPC should be spawned
    UNTARGETABLE,
    INVINCIBLE
}
