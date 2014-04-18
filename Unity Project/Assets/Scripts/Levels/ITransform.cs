using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ITransform
{
    float XRotation { get; set; }
    float YRotation { get; set; }
    float ZRotation { get; set; }
    float X { get; set; }
    float Y { get; set; }
    float Z { get; set; }
    float XScale { get; set; }
    float YScale { get; set; }
    float ZScale { get; set; }
}

public static class ITransformExt
{
    public static void CopyFrom(this ITransform trans, ITransform rhs)
    {
        trans.X = rhs.X;
        trans.Y = rhs.Y;
        trans.Z = rhs.Z;
        trans.XRotation = rhs.XRotation;
        trans.YRotation = rhs.YRotation;
        trans.ZRotation = rhs.ZRotation;
        trans.XScale = rhs.XScale;
        trans.YScale = rhs.YScale;
        trans.ZScale = rhs.ZScale;
    }

    public static void Rotate(this ITransform trans, Rotation rot)
    {
        switch (rot)
        {
            case Rotation.ClockWise:
                trans.YRotation += 90;
                break;
            case Rotation.CounterClockWise:
                trans.YRotation -= 90;
                break;
            case Rotation.OneEighty:
                trans.YRotation += 180;
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
                trans.YRotation = rand.NextBool() ? 90 : -90;
                break;
            case GridDirection.VERT:
                trans.YRotation = rand.NextBool() ? 180 : 0;
                break;
            case GridDirection.DIAGTLBR:
                trans.YRotation = rand.NextBool() ? -45 : 135;
                break;
            case GridDirection.DIAGBLTR:
                trans.YRotation = rand.NextBool() ? 45 : -135;
                break;
        }
    }

    public static void RotateToPoint(this ITransform trans, GridDirection dir, bool TopLeftAffinity = true)
    {
        switch (dir)
        {
            case GridDirection.HORIZ:
                trans.YRotation = TopLeftAffinity ? 90 : -90;
                break;
            case GridDirection.VERT:
                trans.YRotation = TopLeftAffinity ? 0 : 180;
                break;
            case GridDirection.DIAGTLBR:
                trans.YRotation = TopLeftAffinity ? -45 : 135;
                break;
            case GridDirection.DIAGBLTR:
            default:
                trans.YRotation = TopLeftAffinity ? 45 : -135;
                break;
        }
    }

    public static void RotateToPoint(this ITransform trans, GridLocation loc)
    {
        switch (loc)
        {
            case GridLocation.RIGHT:
                trans.YRotation = 90;
                break;
            case GridLocation.TOPRIGHT:
                trans.YRotation = 45;
                break;
            case GridLocation.BOTTOMRIGHT:
                trans.YRotation = 135;
                break;
            case GridLocation.BOTTOM:
                trans.YRotation = 180;
                break;
            case GridLocation.LEFT:
                trans.YRotation = -90;
                break;
            case GridLocation.BOTTOMLEFT:
                trans.YRotation = -135;
                break;
            case GridLocation.TOPLEFT:
                trans.YRotation = -45;
                break;
            case GridLocation.TOP:
            default:
                trans.YRotation = 0;
                break;
        }
    }

    public static void RotateToPoint(this ITransform trans, GridLocation loc, System.Random rand)
    {
        switch (loc)
        {
            case GridLocation.RIGHT:
            case GridLocation.LEFT:
                trans.YRotation = rand.NextBool() ? -90 : 90;
                break;
            case GridLocation.TOPRIGHT:
            case GridLocation.BOTTOMLEFT:
                trans.YRotation = rand.NextBool() ? 45 : -135;
                break;
            case GridLocation.BOTTOMRIGHT:
            case GridLocation.TOPLEFT:
                trans.YRotation = rand.NextBool() ? -45 : 135;
                break;
            case GridLocation.BOTTOM:
            case GridLocation.TOP:
                trans.YRotation = rand.NextBool() ? 0 : 180;
                break;
        }
    }
}