using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ITransform
{
    float Rotation { get; set; }
    float X { get; set; }
    float Y { get; set; }
    float Z { get; set; }
}

public static class ITransformExt
{
    public static void Rotate(this ITransform trans, Rotation rot)
    {
        switch (rot)
        {
            case Rotation.ClockWise:
                trans.Rotation += 90;
                break;
            case Rotation.CounterClockWise:
                trans.Rotation -= 90;
                break;
            case Rotation.OneEighty:
                trans.Rotation += 180;
                break;
            default:
                break;
        }
    }

    public static void RotateToPoint(this ITransform trans, GridDirection dir, System.Random rand)
    {
        switch (dir)
        {
            case GridDirection.HORIZ:
                trans.Rotation = rand.NextBool() ? 90 : -90;
                break;
            case GridDirection.VERT:
                trans.Rotation = rand.NextBool() ? 180 : 0;
                break;
            case GridDirection.DIAGTLBR:
                trans.Rotation = rand.NextBool() ? -45 : 135;
                break;
            case GridDirection.DIAGBLTR:
                trans.Rotation = rand.NextBool() ? 45 : -135;
                break;
        }
    }

    public static void RotateToPoint(this ITransform trans, GridLocation loc)
    {
        switch (loc)
        {
            case GridLocation.RIGHT:
            case GridLocation.TOPRIGHT:
                trans.Rotation = 90;
                break;
            case GridLocation.BOTTOMRIGHT:
            case GridLocation.BOTTOM:
                trans.Rotation = 180;
                break;
            case GridLocation.LEFT:
            case GridLocation.BOTTOMLEFT:
                trans.Rotation = -90;
                break;
            default:
                trans.Rotation = 0;
                break;
        }
    }

    public static void RotateToPoint(this ITransform trans, GridLocation loc, System.Random rand)
    {
        switch (loc)
        {
            case GridLocation.RIGHT:
            case GridLocation.TOPRIGHT:
            case GridLocation.LEFT:
            case GridLocation.BOTTOMLEFT:
                trans.Rotation = rand.NextBool() ? -90 : 90;
                break;
            case GridLocation.BOTTOMRIGHT:
            case GridLocation.BOTTOM:
            case GridLocation.TOPLEFT:
            case GridLocation.TOP:
                trans.Rotation = rand.NextBool() ? 0 : 180;
                break;
        }
    }
}