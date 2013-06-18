using System;

[Flags]
enum NPCProperties
{
    //store the NPC's properties here
    //ex:
    NONE = 0x0,
    LEVITATION = 0x1,
    FLYING = 0x2,
    TELEPATHY = 0x4,
    SWIMMING = 0x8,
    SEE_INVIS = 0x10,
    WATERWALK = 0x20,
    BREATHLESS = 0x40,
    INFRAVISION = 0x80,
}