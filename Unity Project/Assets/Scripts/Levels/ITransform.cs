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
}