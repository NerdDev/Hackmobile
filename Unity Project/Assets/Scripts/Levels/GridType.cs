using System.Collections;

public enum GridType
{
    // Order represents "priority" if two are in the same spot, which to display
    NULL,
	INTERNAL_RESERVED_BLOCKED,
	
    Floor,
    PathFloor,
    Wall,
    Door,
    TrapDoor,
	
	INTERNAL_RESERVED_REJECTED,
	INTERNAL_RESERVED_HORIZ,
	INTERNAL_RESERVED_VERT,
	INTERNAL_RESERVED_LT,
	INTERNAL_RESERVED_LB,
	INTERNAL_RESERVED_RT,
	INTERNAL_RESERVED_RB,
	INTERNAL_RESERVED_ACCEPTED,
	INTERNAL_RESERVED_CUR
}
