using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class GridTypeDrawExt
{
    public static void DrawPotentialDoors(this Container2D<GridType> arr, DrawActionCall<GridType> action)
    {
        DrawActionCall<GridType> check = Draw.CanDrawDoor();
        arr.DrawSquare(new DrawActionCall<GridType>((arr2, x, y) =>
            {
                if (check(arr2, x, y))
                    action(arr2, x, y);
                return true;
            }));
    }

    public static void DrawPotentialDoors(this Container2D<GridType> arr, StrokedAction<GridType> action)
    {
        DrawActionCall<GridType> check = Draw.CanDrawDoor();
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

        arr.DrawPerimeter((arr2, x, y) =>
            {
                return arr2[x, y] != GridType.NULL;
            },
            findDoors
            );
    }

    public static void DrawPotentialExternalDoors(this Container2D<GridType> arr, DrawActionCall<GridType> action)
    {
        DrawPotentialDoors(arr, new StrokedAction<GridType>() { StrokeAction = action });
    }

    public static void DrawPotentialInternalDoors(this Container2D<GridType> arr, DrawActionCall<GridType> action)
    {
        DrawPotentialDoors(arr, new StrokedAction<GridType>() { UnitAction = action });
    }
}
