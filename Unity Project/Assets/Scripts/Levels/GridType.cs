using System.Collections;

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
