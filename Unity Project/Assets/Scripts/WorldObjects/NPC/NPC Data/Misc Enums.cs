/**
 * Stick all the smaller enums in here for organization?
 */

public enum HungerLevel
{
    Stuffed,
    Satiated,
    Hungry,
    Starving,
    Faint
}

public enum EncumbranceLevel  //speed and to-hit mods will be affected.  Nethack "exercises" strength and "abuses" dex/const based on this enum
{
    Unencumbered,
    Burdened,
    Stressed,
    Strained,
    Overtaxed,
    Overloaded
}