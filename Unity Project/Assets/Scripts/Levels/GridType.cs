using System.Collections;
using System;

public enum GridType
{
    NULL,
    INTERNAL_RESERVED_BLOCKED,

    Floor,
    Wall,
    Doodad,
    Door,
    TrapDoor,
    StairUp,
    StairDown,
    StairPlace,

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

    INTERNAL_RESERVED_CUR,
    INTERNAL_MARKER_1,
    INTERNAL_MARKER_2,
    INTERNAL_MARKER_FRIENDLY,
    INTERNAL_RATIO_LOW,
    INTERNAL_RATIO_MED,
    INTERNAL_RATIO_HIGH
}

public class GridTypeEnum
{
    public static Func<GridType, char> CharConverter = new Func<GridType, char>((g) =>
    {
        return Convert(g);
    });
    public static char Convert(GridType g)
    {
        switch (g)
        {
            case GridType.Floor:
                return ' ';
            case GridType.TrapDoor:
                return 'T';
            case GridType.Trap:
                return 'X';
            case GridType.Door:
                return ':';
            case GridType.SmallLoot:
                return '$';
            case GridType.Chest:
                return (char)227;
            case GridType.Wall:
                return (char)178;
            case GridType.StairUp:
                return '/';
            case GridType.StairDown:
                return '\\';
            case GridType.StairPlace:
                return (char)167;
            case GridType.NULL:
                return (char)219;
            case GridType.INTERNAL_RESERVED_BLOCKED:
                return '*';
            case GridType.INTERNAL_RESERVED_CUR:
                return '%';
            case GridType.INTERNAL_MARKER_1:
                return (char)233;
            case GridType.INTERNAL_MARKER_2:
                return (char)143;
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
            case GridType.INTERNAL_RATIO_LOW:
                return (char)176;
            case GridType.INTERNAL_RATIO_MED:
                return (char)177;
            case GridType.INTERNAL_RATIO_HIGH:
                return (char)178;
            case GridType.INTERNAL_MARKER_FRIENDLY:
                return 'F';
            case GridType.Enemy:
                return 'E';
            case GridType.Doodad:
                return (char)239;
            default:
                return '?';
        };
    }

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
            case GridType.Door:
            case GridType.StairPlace:
                return true;
            default:
                return false;
        }
    }

    public static bool EdgeType(GridType t)
    {
        switch (t)
        {
            case GridType.Wall:
            case GridType.Door:
                return true;
            default:
                return false;
        }
    }

    public static bool WallType(GridType t)
    {
        switch (t)
        {
            case GridType.Wall:
                return true;
            default:
                return false;
        }
    }

    public static bool FloorType(GridType t)
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
            case GridType.StairPlace:
                return true;
            default:
                return false;
        }
    }
}
