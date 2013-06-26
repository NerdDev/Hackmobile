using System;

public enum Priority
{
    NEGITEM,
    NEGCRITICAL = -4,
    NEGHIGH = -3,
    NEGMEDIUM = -2,
    NEGLOW = -1,
    NONE = 0,
    LOW = 1,
    MEDIUM = 2,
    HIGH = 3,
    CRITICAL = 4,
    ITEM,
}