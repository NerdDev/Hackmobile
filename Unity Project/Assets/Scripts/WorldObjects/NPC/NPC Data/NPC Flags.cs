using System;

[FlagsAttribute]
public enum NPCFlags
{
    //store specific flags for NPC's here
    //These would be affecting how and what the NPC does (more under the hood)
    //  while the NPC properties stores variables like levitation
    //  which are properties of the NPC itself
    NONE,
    LEAVES_CORPSE,
    COVETOUS,
    NOPOLY, //player can't turn into one of these
    GREEDY,
    //etc
}