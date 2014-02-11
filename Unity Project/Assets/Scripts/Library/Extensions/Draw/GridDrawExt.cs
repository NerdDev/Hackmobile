using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class GridTypeDrawExt
{
    public static void DrawPotentialDoors(this Container2D<GridType> arr, DrawAction<GridType> action)
    {
        DrawAction<GridType> check = Draw.CanDrawDoor();
        arr.DrawAll(new DrawAction<GridType>((arr2, x, y) =>
            {
                if (check(arr2, x, y))
                    action(arr2, x, y);
                return true;
            }));
    }

    public static void DrawPotentialDoors(this Container2D<GridType> arr, StrokedAction<GridType> action)
    {
        DrawAction<GridType> check = Draw.CanDrawDoor();
        StrokedAction<GridType> findDoors = new StrokedAction<GridType>();
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

        arr.DrawPerimeter(Draw.Not(Draw.EqualTo(GridType.NULL)), findDoors);
    }

    public static void DrawPotentialExternalDoors(this Container2D<GridType> arr, DrawAction<GridType> action)
    {
        DrawPotentialDoors(arr, new StrokedAction<GridType>() { StrokeAction = action });
    }

    public static void DrawPotentialInternalDoors(this Container2D<GridType> arr, DrawAction<GridType> action)
    {
        DrawPotentialDoors(arr, new StrokedAction<GridType>() { UnitAction = action });
    }

    public static ProbabilityList<int> DoorRatioPicker;
    public static void PlaceSomeDoors(this Container2D<GridType> arr, IEnumerable<Point> points, System.Random rand, Point shift = null)
    {
        MultiMap<GridType> acceptablePoints = new MultiMap<GridType>();
        Counter numPoints = new Counter();
        DrawAction<GridType> call = Draw.Count<GridType>(numPoints).And(Draw.CanDrawDoor().IfThen(Draw.AddTo(acceptablePoints)));
        if (shift != null)
        {
            call = call.Shift<GridType>(shift.x, shift.y);
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
        numDoors += DoorRatioPicker.Get();
        if (numDoors <= 0)
            numDoors = 1;
        foreach (Point picked in acceptablePoints.Random(rand, numDoors, 1))
        {
            arr[picked] = GridType.Door;
        }
    }
}
