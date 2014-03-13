using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MassTombRoom : HeavyRoomMod
{
    const int MIN_TOMBS = 3;

    protected override bool ModifyInternal(RoomSpec spec)
    {
        UndeadTombTheme undeadTheme = spec.Theme as UndeadTombTheme;
        if (undeadTheme == null) throw new ArgumentException("Theme needs to be undead themed.");
        ThemeElement[] tombCollection = undeadTheme.Tombs.Random(spec.Random).Elements;
        ThemeElement tombProto = tombCollection[0];
        List<List<Bounding>> options = spec.Grids.FindRectanglesMaximized(tombProto.GridWidth + 2, tombProto.GridHeight + 2, true, new StrokedAction<GenSpace>()
            {
                UnitAction = Draw.IsType<GenSpace>(GridType.Floor),
                StrokeAction = Draw.Walkable<GenSpace>()
            }, spec.Grids.Bounding);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Printing tomb layout options");
            for (int i = 0 ; i < options.Count ; i++)
            {
                MultiMap<GenSpace> tmp = new MultiMap<GenSpace>(spec.Grids);
                foreach (Bounding bound in options[i])
                {
                    tmp.DrawRect(bound.XMin, bound.XMax, bound.YMin, bound.YMax, new StrokedAction<GenSpace>()
                    {
                        UnitAction = Draw.SetTo(GridType.Wall, spec.Theme),
                        StrokeAction = Draw.Nothing<GenSpace>()
                    });
                }
                tmp.ToLog(Logs.LevelGen, "Option " + i);
            }
            BigBoss.Debug.printFooter(Logs.LevelGen, "Printing tomb layout options");
        }
        #endregion
        while (options.Count > 0)
        {
            List<Bounding> set = options.RandomTake(spec.Random);
            if (set.Count > MIN_TOMBS)
            {
                foreach (Bounding tombBound in set)
                {
                    GenDeploy tomb = new GenDeploy(tombCollection.Random(spec.Random));
                    spec.Grids.DrawRect(tombBound.XMin, tombBound.XMax, tombBound.YMin, tombBound.YMax, new StrokedAction<GenSpace>()
                        {
                            UnitAction = Draw.MergeIn(tomb, spec.Theme),
                            StrokeAction = Draw.Nothing<GenSpace>()
                        });
                }
                return true;
            }
        }
        return false;
    }
}

