using System.Collections;
using System;

public enum GridType
{
    NULL,
    INTERNAL_RESERVED_BLOCKED,

    Floor,
    Wall,
    Door,
    TrapDoor,

    Path_Horiz,
    Path_Vert,
    Path_LT,
    Path_LB,
    Path_RT,
    Path_RB,

    Enemy,
    Trap,
    Secret,
    SmallLoot,
    Chest,

    INTERNAL_RESERVED_CUR
}

public class GridTypeEnum
{
    public static Func<GridType, char> CharConverter = new Func<GridType, char>((g) =>
    {
        switch (g)
        {
            case GridType.Floor:
                return '.';
            case GridType.TrapDoor:
                return 'T';
            case GridType.Door:
                return '|';
            case GridType.Wall:
                return '#';
            case GridType.NULL:
                return ' ';
            case GridType.INTERNAL_RESERVED_BLOCKED:
                return '*';
            case GridType.INTERNAL_RESERVED_CUR:
                return '%';
            case GridType.Path_Horiz:
                return (char)205;
            case GridType.Path_Vert:
                return (char)186;
            case GridType.Path_LT:
                return (char)188;
            case GridType.Path_LB:
                return (char)187;
            case GridType.Path_RT:
                return (char)200;
            case GridType.Path_RB:
                return (char)201;
            default:
                return '?';
        }
    });

    public static bool Walkable(GridType t)
    {
        switch (t)
        {
            case GridType.Floor:
            case GridType.Path_Horiz:
            case GridType.Path_Vert:
            case GridType.Path_LT:
            case GridType.Path_LB:
            case GridType.Path_RT:
            case GridType.Path_RB:
            case GridType.TrapDoor:
            case GridType.Trap:
            case GridType.SmallLoot:
            case GridType.Chest:
                return true;
            default:
                return false;
        }
    }
}
