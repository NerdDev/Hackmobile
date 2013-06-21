using System.Collections;

public enum GridType
{
    // Order represents "priority" if two are in the same spot, which to display
    NULL,
	INTERNAL_RESERVED_BLOCKED,
	
    Floor,
    Wall,
    Door,
    TrapDoor,
	
	INTERNAL_RESERVED_REJECTED,
	Path_Horiz,
	Path_Vert,
	Path_LT,
	Path_LB,
	Path_RT,
	Path_RB,
	INTERNAL_RESERVED_ACCEPTED,
	INTERNAL_RESERVED_CUR
}
