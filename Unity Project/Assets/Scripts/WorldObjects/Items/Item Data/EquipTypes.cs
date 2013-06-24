using System;

public enum EquipTypes
{
    NONE = 10, //general default for non-equippable items
    LEGS = 0, //pants and etc
    BODY = 1, //plate armor, torso stuff
    HEAD = 2, //helmets and etc
    SHIRT = 3, //under the torso? do we still need this?
    NECK = 4, //amulets
    FEET = 5, //for boots
    HAND = 6, // for weapons/shields/anything requiring a hand
    RING = 7, //for rings, generically cased
    RIGHT_RING = 8, //right ring
    LEFT_RING = 9, //left ring
}