using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Targeter to target every object in a radius around each point given
 */
public class AOELocation : AOE
{
    public override HashSet<IAffectable> GetTargets(SpellCastInfo castInfo)
    {
        GridSpace[,] level = BigBoss.Levels.Level.Array;
        var targetSpaces = new HashSet<GridSpace>();
        foreach (GridSpace point in castInfo.TargetSpaces)
            level.DrawCircle(point.X, point.Y, Radius, Draw.AddTo(targetSpaces));
        castInfo.TargetSpaces = targetSpaces.ToList();
        return base.GetTargets(castInfo);
    }
}
