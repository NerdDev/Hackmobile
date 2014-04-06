using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class GridTypeDrawExt
{
    public static void DrawPotentialDoors(this Container2D<GenSpace> arr, DrawAction<GenSpace> action)
    {
        DrawAction<GenSpace> check = Draw.CanDrawDoor();
        arr.DrawAll(new DrawAction<GenSpace>((arr2, x, y) =>
            {
                if (check(arr2, x, y))
                    action(arr2, x, y);
                return true;
            }));
    }

    public static void DrawPotentialDoors(this Container2D<GenSpace> arr, StrokedAction<GenSpace> action)
    {
        DrawAction<GenSpace> check = Draw.CanDrawDoor();
        StrokedAction<GenSpace> findDoors = new StrokedAction<GenSpace>();
        if (action.StrokeAction != null)
        {
            findDoors.StrokeAction = (arr2, x, y) =>
                {
                    if (check(arr2, x, y))
                        action.StrokeAction(arr2, x, y);
                    return true;
                };
        }
        if (action.UnitAction != null)
        {
            findDoors.UnitAction = (arr2, x, y) =>
                {
                    if (check(arr2, x, y))
                        action.UnitAction(arr2, x, y);
                    return true;
                };
        }

        arr.DrawPerimeter(Draw.Not(Draw.IsType<GenSpace>(GridType.NULL)), findDoors);
    }

    public static void DrawPotentialExternalDoors(this Container2D<GenSpace> arr, DrawAction<GenSpace> action)
    {
        DrawPotentialDoors(arr, new StrokedAction<GenSpace>() { StrokeAction = action });
    }

    public static void DrawPotentialInternalDoors(this Container2D<GenSpace> arr, DrawAction<GenSpace> action)
    {
        DrawPotentialDoors(arr, new StrokedAction<GenSpace>() { UnitAction = action });
    }

    public static ProbabilityList<int> DoorRatioPicker;
    public static List<Value2D<GenSpace>> PlaceSomeDoors(this Container2D<GenSpace> arr, IEnumerable<Point> points, Theme theme, System.Random rand, int desiredWallToDoorRatio = -1, Point shift = null)
    {
        if (desiredWallToDoorRatio < 0)
        {
            desiredWallToDoorRatio = LevelGenerator.desiredWallToDoorRatio;
        }
        var acceptablePoints = new MultiMap<GenSpace>();
        Counter numPoints = new Counter();
        DrawAction<GenSpace> call = Draw.Count<GenSpace>(numPoints).And(Draw.CanDrawDoor().IfThen(Draw.AddTo(acceptablePoints)));
        if (shift != null)
        {
            call = call.Shift<GenSpace>(shift.x, shift.y);
        }
        arr.DrawPoints(points, call);
        if (DoorRatioPicker == null)
        {
            DoorRatioPicker = new ProbabilityList<int>();
            DoorRatioPicker.Add(-2, .25);
            DoorRatioPicker.Add(-1, .5);
            DoorRatioPicker.Add(0, 1);
            DoorRatioPicker.Add(1, .5);
            DoorRatioPicker.Add(2, .25);
        }
        int numDoors = numPoints / desiredWallToDoorRatio;
        numDoors += DoorRatioPicker.Get(rand);
        if (numDoors <= 0)
            numDoors = 1;
        List<Value2D<GenSpace>> pickedPts = acceptablePoints.GetRandom(rand, numDoors, 1);
        foreach (Point picked in pickedPts)
        {
            arr.SetTo(picked, GridType.Door, theme);
        }
        return pickedPts;
    }
}
