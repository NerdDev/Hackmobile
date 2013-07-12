using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FilterDFS : PassFilter<Value2D<GridType>>
{
    Array2D<bool> blocked;
    HashSet<GridType> targets;

    public FilterDFS(Array2D<bool> blocked, HashSet<GridType> targets)
    {
        this.blocked = blocked;
        this.targets = targets;
    }

    public bool pass(Value2D<GridType> dir)
    {
        return dir != null 
            && !blocked[dir.x, dir.y] // If not blocked
            && (dir.val == GridType.NULL || targets.Contains(dir.val)); // If open space or target
    }
}
