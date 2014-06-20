using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ShiftIntoPlaceTest : TestLevelSetup
{
    public SmartThemeElement ShiftElement;

    public override LevelLayout Create()
    {
        LevelLayout layout = new LevelLayout();
        layout.DrawRect(-3, 3, -3, 20, Draw.SetTo(GridType.Floor, BigBoss.Levels.TestTheme));
        GenDeploy deploy = new GenDeploy(ShiftElement.Get(Probability.Rand));
        deploy.ColliderDeploy = ColliderDeploy.Destroy;
        deploy.ColliderPlacementQueue = new[] { AxisDirection.ZNeg };
        deploy.DelayDeployment = true;
        layout.MergeIn(-3, -3, deploy, BigBoss.Levels.TestTheme);
        return layout;
    }

    public override void Spawn(Level level)
    {
    }
}

