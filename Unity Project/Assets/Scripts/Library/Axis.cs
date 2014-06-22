using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum Axis
{
    X,
    Y,
    Z
}

public enum AxisDirection
{
    X,
    XNeg,
    Y,
    YNeg,
    Z,
    ZNeg,
    XY,
    XYNeg,
    XNegY,
    XNegYNeg,
    YZ,
    YZNeg,
    YNegZ,
    YNegZNeg,
    XZ,
    XZNeg,
    XNegZ,
    XNegZNeg
}

public static class AxisDirectionExt
{
    public static Vector3 X = new Vector3(1, 0, 0);
    public static Vector3 XNeg = new Vector3(-1, 0, 0);
    public static Vector3 Y = new Vector3(0, 1, 0);
    public static Vector3 YNeg = new Vector3(0, -1, 0);
    public static Vector3 Z = new Vector3(0, 0, 1);
    public static Vector3 ZNeg = new Vector3(0, 0, -1);
    public static Vector3 XY = new Vector3(1, 1, 0);
    public static Vector3 XYNeg = new Vector3(1, -1, 0);
    public static Vector3 XNegY = new Vector3(-1, 1, 0);
    public static Vector3 XNegYNeg = new Vector3(-1, -1, 0);
    public static Vector3 YZ = new Vector3(0, 1, 1);
    public static Vector3 YZNeg = new Vector3(0, 1, -1);
    public static Vector3 YNegZ = new Vector3(0, -1, 1);
    public static Vector3 YNegZNeg = new Vector3(0, -1, -1);
    public static Vector3 XZ = new Vector3(1, 0, 1);
    public static Vector3 XZNeg = new Vector3(1, 0, -1);
    public static Vector3 XNegZ = new Vector3(-1, 0, 1);
    public static Vector3 XNegZNeg = new Vector3(-1, 0, -1);
    public static Vector3 GetVector3(this AxisDirection dir)
    {
        switch (dir)
        {
            case AxisDirection.X:
                return X;
            case AxisDirection.XNeg:
                return XNeg;
            case AxisDirection.Y:
                return Y;
            case AxisDirection.YNeg:
                return YNeg;
            case AxisDirection.Z:
                return Z;
            case AxisDirection.ZNeg:
                return ZNeg;
            case AxisDirection.XY:
                return XY;
            case AxisDirection.XYNeg:
                return XYNeg;
            case AxisDirection.XNegY:
                return XNegY;
            case AxisDirection.XNegYNeg:
                return XNegYNeg;
            case AxisDirection.YZ:
                return YZ;
            case AxisDirection.YZNeg:
                return YZNeg;
            case AxisDirection.YNegZ:
                return YNegZ;
            case AxisDirection.YNegZNeg:
                return YNegZNeg;
            case AxisDirection.XZ:
                return XZ;
            case AxisDirection.XZNeg:
                return XZNeg;
            case AxisDirection.XNegZ:
                return XNegZ;
            case AxisDirection.XNegZNeg:
            default:
                return XNegZNeg;
        }
    }
}
