using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class GridTypeDrawExt
{
    public static void DrawPotentialDoors(this Container2D<GridSpace> arr, DrawAction<GridSpace> action)
    {
        DrawAction<GridSpace> check = Draw.CanDrawDoor();
        arr.DrawAll(new DrawAction<GridSpace>((arr2, x, y) =>
            {
                if (check(arr2, x, y))
                    action(arr2, x, y);
                return true;
            }));
    }

    public static void DrawPotentialDoors(this Container2D<GridSpace> arr, StrokedAction<GridSpace> action)
    {
        DrawAction<GridSpace> check = Draw.CanDrawDoor();
        StrokedAction<GridSpace> findDoors = new StrokedAction<GridSpace>();
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

        arr.DrawPerimeter(Draw.Not(Draw.IsType(GridType.NULL)), findDoors);
    }

    public static void DrawPotentialExternalDoors(this Container2D<GridSpace> arr, DrawAction<GridSpace> action)
    {
        DrawPotentialDoors(arr, new StrokedAction<GridSpace>() { StrokeAction = action });
    }

    public static void DrawPotentialInternalDoors(this Container2D<GridSpace> arr, DrawAction<GridSpace> action)
    {
        DrawPotentialDoors(arr, new StrokedAction<GridSpace>() { UnitAction = action });
    }

    public static ProbabilityList<int> DoorRatioPicker;
    public static List<Value2D<GridSpace>> PlaceSomeDoors(this Container2D<GridSpace> arr, IEnumerable<Point> points, System.Random rand, Point shift = null)
    {
        var acceptablePoints = new MultiMap<GridSpace>();
        Counter numPoints = new Counter();
        DrawAction<GridSpace> call = Draw.Count<GridSpace>(numPoints).And(Draw.CanDrawDoor().IfThen(Draw.AddTo(acceptablePoints)));
        if (shift != null)
        {
            call = call.Shift<GridSpace>(shift.x, shift.y);
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
        int numDoors = numPoints / LevelGenerator.desiredWallToDoorRatio;
        numDoors += DoorRatioPicker.Get(rand);
        if (numDoors <= 0)
            numDoors = 1;
        List<Value2D<GridSpace>> pickedPts = acceptablePoints.GetRandom(rand, numDoors, 1);
        foreach (Point picked in pickedPts)
        {
            arr.SetTo(picked.x, picked.y, GridType.Door);
        }
        return pickedPts;
    }
}
