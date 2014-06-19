using UnityEngine;
using System.Collections;

public class SpikeClusterTest : TestLevelSetup
{
    public ClusterElement SpikeClusterElement;

    public override LevelLayout Create()
    {
        LevelLayout layout = new LevelLayout();
        layout.DrawRect(-10, 10, -10, 10, Draw.SetTo(GridType.Floor, BigBoss.Levels.TestTheme));
        GenDeploy deploy = new GenDeploy(SpikeClusterElement);
        layout.MergeIn(1, 1, deploy, BigBoss.Levels.TestTheme);
        GenDeploy deploy1 = new GenDeploy(SpikeClusterElement);
        layout.MergeIn(1, 2, deploy1, BigBoss.Levels.TestTheme);
        GenDeploy deploy2 = new GenDeploy(SpikeClusterElement);
        layout.MergeIn(2, 1, deploy2, BigBoss.Levels.TestTheme);
        return layout;
    }

    public override void Spawn(Level level)
    {
    }
}
