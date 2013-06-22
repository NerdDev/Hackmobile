using System;

[FlagsAttribute]
public enum NPCFlags
{
    //store specific flags for NPC's here
    //These would be affecting how and what the NPC does (more under the hood)
    //  while the NPC properties stores variables like levitation
    //  which are properties of the NPC itself
    NONE = 0x0,
    LEAVES_CORPSE = 0x1,
    COVETOUS = 0x2,
    NOPOLY = 0x4, //player can't turn into one of these
    GREEDY = 0x8,
    //etc
}