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
        deploy.Y = 2;
        deploy.ColliderDeploy = ColliderDeploy.Destroy;
        deploy.AddShiftInstruction(new ShiftPlacementInstruction()
        {
            Direction = AxisDirection.YNeg,
            LookLength = 10F
        });
        deploy.AddShiftInstruction(new ShiftPlacementInstruction()
        {
            Direction = AxisDirection.ZNeg
        });
        deploy.AddShiftInstruction(new ShiftPlacementInstruction()
        {
            Direction = AxisDirection.XNeg
        });
        deploy.DelayDeployment = true;
        layout.MergeIn(-3, -3, deploy, BigBoss.Levels.TestTheme);
        // 2nd
        deploy = new GenDeploy(ShiftElement.Get(Probability.Rand));
        deploy.Y = 2;
        deploy.ColliderDeploy = ColliderDeploy.Destroy;
        deploy.AddShiftInstruction(new ShiftPlacementInstruction()
        {
            Direction = AxisDirection.YNeg,
            LookLength = 10F
        });
        deploy.AddShiftInstruction(new ShiftPlacementInstruction()
        {
            Direction = AxisDirection.ZNeg
        });
        deploy.AddShiftInstruction(new ShiftPlacementInstruction()
        {
            Direction = AxisDirection.XNeg
        });
        deploy.DelayDeployment = true;
        layout.MergeIn(-3, -3, deploy, BigBoss.Levels.TestTheme);
        // 3rd
        //deploy = new GenDeploy(ShiftElement.Get(Probability.Rand));
        //deploy.Y = 2;
        //deploy.ColliderDeploy = ColliderDeploy.Destroy;
        //deploy.AddShiftInstruction(new ShiftPlacementInstruction()
        //{
        //    Direction = AxisDirection.YNeg,
        //    LookLength = 10F
        //});
        //deploy.AddShiftInstruction(new ShiftPlacementInstruction()
        //{
        //    Direction = AxisDirection.XNeg
        //});
        //deploy.AddShiftInstruction(new ShiftPlacementInstruction()
        //{
        //    Direction = AxisDirection.ZNeg
        //});
        //deploy.DelayDeployment = true;
        //layout.MergeIn(-3, -3, deploy, BigBoss.Levels.TestTheme);
        return layout;
    }

    public override void Spawn(Level level)
    {
    }
}

