using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class FleeTest : TestLevelSetup
{
    public override LevelLayout Create()
    {
        LevelLayout layout = new LevelLayout();
        layout.DrawRect(-1, 1, -10, 10, Draw.SetTo(GridType.Floor, BigBoss.Levels.TestTheme));
        layout.DrawRect(-1, 15, -1, 1, Draw.SetTo(GridType.Floor, BigBoss.Levels.TestTheme));
        layout.DrawCircle(0, 0, 5, Draw.SetTo(GridType.Floor, BigBoss.Levels.TestTheme));
        return layout;
    }

    public override void Spawn(Level level)
    {
        BigBoss.DungeonMaster.SpawnNPC(level[14, 0], "Giant Rat");
    }
}
