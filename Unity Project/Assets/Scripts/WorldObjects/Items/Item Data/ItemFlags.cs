using System;

[Flags]
public enum ItemFlags
{
    NONE = 0x0,
    IS_EDIBLE = 0x1,
    IS_EQUIPPED = 0x2,
    DEFAULT,
}