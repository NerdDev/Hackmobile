using System;

public enum Properties : long
{
    //store the NPC's properties here
    //ex:
    NONE = 0, 
    LEVITATION,
    FLYING,
    TELEPATHY,
    SWIMMING,
    SEE_INVIS,
    WATERWALK,
    BREATHLESS,
    INFRAVISION,

    //resistances
    FIRE_RES,
    ACID_RES,
    SLEEP_RES,
    COLD_RES,
    STONE_RES,
    POISON_RES,
    SHOCK_RES,
    DISINT_RES,
    MAGIC_RES,
    HALLU_RES,
    DISEASE_RES,
    DRAIN_RES,

    //detriments
    POISONED,

    LAST, //this is a marker of the number of properties in the enum

    //Duplicates for clarity
    HEALTHLOSS = POISONED,
}