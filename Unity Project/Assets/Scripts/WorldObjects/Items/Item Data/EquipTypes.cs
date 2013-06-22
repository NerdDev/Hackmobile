using System;

[Flags]
public enum EquipTypes
{
    NONE = 0x0, //general default for non-equippable items
    RIGHT_HAND = 0x1, //right ring
    LEFT_HAND = 0x2, //left ring
    LEGS = 0x4, //pants and etc
    BODY = 0x8, //plate armor, torso stuff
    HEAD = 0x10, //helmets and etc
    SHIRT = 0x20, //under the torso? do we still need this?
    NECK = 0x40, //amulets
    FEET = 0x80, //for boots
    HAND = 0x100, // for generic hand ring slots
}