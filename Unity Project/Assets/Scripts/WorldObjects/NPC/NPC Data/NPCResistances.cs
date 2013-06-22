using System;

[Flags]
enum NPCResistances
{
    //store resistances here
    //ex:
    NONE = 0x0,
    FIRE_RES = 0x1,
    ACID_RES = 0x2,
    SLEEP_RES = 0x4,
    COLD_RES = 0x8,
    STONE_RES = 0x10,
    POISON_RES = 0x20,
    SHOCK_RES = 0x40,
    DISINT_RES = 0x80,
    MAGIC_RES = 0x100,
    HALLU_RES = 0x200,
    DISEASE_RES = 0x400,
    DRAIN_RES = 0x800,
}