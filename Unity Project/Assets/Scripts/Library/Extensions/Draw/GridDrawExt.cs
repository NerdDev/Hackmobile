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
}
