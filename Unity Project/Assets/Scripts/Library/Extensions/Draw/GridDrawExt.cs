using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class GridTypeDrawExt
{
    public static void DrawPotentialDoors<T>(this Container2D<T> arr, DrawAction<T> action)
        where T : IGridSpace
    {
        DrawAction<T> check = Draw.CanDrawDoor<T>();
        arr.DrawAll(new DrawAction<T>((arr2, x, y) =>
            {
                if (check(arr2, x, y))
                    action(arr2, x, y);
                return true;
            }));
    }

    public static void DrawPotentialDoors<T>(this Container2D<T> arr, StrokedAction<T> action)
        where T : IGridSpace
    {
        DrawAction<T> check = Draw.CanDrawDoor<T>();
        StrokedAction<T> findDoors = new StrokedAction<T>();
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

        arr.DrawPerimeter(Draw.Not(Draw.IsType<T>(GridType.NULL)), findDoors);
    }

    public static void DrawPotentialExternalDoors<T>(this Container2D<T> arr, DrawAction<T> action)
        where T : IGridSpace
    {
        DrawPotentialDoors(arr, new StrokedAction<T>() { StrokeAction = action });
    }

    public static void DrawPotentialInternalDoors<T>(this Container2D<T> arr, DrawAction<T> action)
        where T : IGridSpace
    {
        DrawPotentialDoors(arr, new StrokedAction<T>() { UnitAction = action });
    }

    public static ProbabilityList<int> DoorRatioPicker;
    public static List<Value2D<GenSpace>> PlaceSomeDoors(this Container2D<GenSpace> arr, IEnumerable<Point> points, Theme theme, System.Random rand, Point shift = null)
    {
        var acceptablePoints = new MultiMap<GenSpace>();
        Counter numPoints = new Counter();
        DrawAction<GenSpace> call = Draw.Count<GenSpace>(numPoints).And(Draw.CanDrawDoor<GenSpace>().IfThen(Draw.AddTo(acceptablePoints)));
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
        int numDoors = numPoints / LevelGenerator.desiredWallToDoorRatio;
        numDoors += DoorRatioPicker.Get(rand);
        if (numDoors <= 0)
            numDoors = 1;
        List<Value2D<GenSpace>> pickedPts = acceptablePoints.GetRandom(rand, numDoors, 1);
        foreach (Point picked in pickedPts)
        {
            arr.SetTo(picked.x, picked.y, new GenSpace(GridType.Door, theme));
        }
        return pickedPts;
    }
}
