using UnityEngine;
using System.Collections;

public class HordeTest : TestLevelSetup
{
    public override LevelLayout Create()
    {
        LevelLayout layout = new LevelLayout();
        layout.DrawRect(-1, 1, -10, 10, Draw.SetTo(GridType.Floor, BigBoss.Levels.TestTheme));
        layout.DrawRect(-10, 15, -4, -2, Draw.SetTo(GridType.Floor, BigBoss.Levels.TestTheme));
        layout.DrawRect(-10, -8, -10, 10, Draw.SetTo(GridType.Floor, BigBoss.Levels.TestTheme));
        return layout;
    }

    public override void Spawn(Level level)
    {
        BigBoss.DungeonMaster.SpawnNPC(level[0, -9], "Giant Rat");
        BigBoss.DungeonMaster.SpawnNPC(level[0, 9], "Giant Rat");
        BigBoss.DungeonMaster.SpawnNPC(level[14, -3], "Giant Rat");
        BigBoss.DungeonMaster.SpawnNPC(level[-9, 9], "Giant Rat");
        BigBoss.DungeonMaster.SpawnNPC(level[-9, -9], "Giant Rat");
    }
}
