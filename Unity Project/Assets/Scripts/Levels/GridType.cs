using System.Collections;

public enum GridType
{
    // Order represents "priority" if two are in the same spot, which to display
    NULL,
    Floor,
    PathFloor,
    Wall,
    Door,
    TrapDoor,
	
	INTERNAL_RESERVED_REJECTED,
	INTERNAL_RESERVED_ACCEPTED,
	INTERNAL_RESERVED_BLOCKED,
	INTERNAL_RESERVED_CUR
}
