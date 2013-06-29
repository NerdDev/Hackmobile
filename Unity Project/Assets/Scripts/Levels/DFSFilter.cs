using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DFSFilter : PassFilter<Value2D<GridType>>
{
    Array2D<bool> blocked;
    HashSet<GridType> targets;

    public DFSFilter(Array2D<bool> blocked, HashSet<GridType> targets)
    {
        this.blocked = blocked;
        this.targets = targets;
    }

    public bool pass(Value2D<GridType> dir)
    {
        return !blocked.Get(dir.x, dir.y) // If not blocked
           && (dir.val == GridType.NULL || targets.Contains(dir.val)); // If open space or target
    }
}
