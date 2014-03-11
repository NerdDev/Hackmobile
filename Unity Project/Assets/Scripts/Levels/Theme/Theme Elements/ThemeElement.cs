using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ThemeElement : MonoBehaviour
{
    public GameObject GO { get { return gameObject; } }
    private Bounds _bounds;
    private bool _bounded = false;
    public Bounds Bounds
    {
        get
        {
            if (!_bounded)
            {
                foreach (Renderer render in GetComponentsInChildren<Renderer>(true))
                {
                    _bounds.Encapsulate(render.bounds);
                }
                _bounded = true;
            }
            return _bounds;
        }
    }
    public byte GridWidth = 1;
    public byte GridHeight = 1;
    public string PrintChar = string.Empty;

    public virtual MultiMap<List<GenDeploy>> PreDeployTweaks(ThemeElementSpec spec)
    {
        return null;
    }

    public MultiMap<List<GenDeploy>> PlaceFloors(ThemeElementSpec spec)
    {
        var ret = new MultiMap<List<GenDeploy>>();
        foreach (var space in spec)
        {
            GenDeploy deploy = new GenDeploy(spec.Theme.Floor.Random(spec.Random));
            deploy.AddSpace(space.val, space.x, space.y);
            List<GenDeploy> list;
            if (!ret.TryGetValue(space, out list))
            {
                list = new List<GenDeploy>(1);
                ret[space] = list;
            }
            list.Add(deploy);
        }
        return ret;
    }

    protected void CenterDoodad(ThemeElementSpec spec)
    {
        if (spec.GenDeploy.Element.GridHeight == 1
            && spec.GenDeploy.Element.GridWidth == 1) return;
        Bounding bounds = spec.GetBounds();
        Point center = bounds.GetCenter();
        center.x -= spec.X;
        center.y -= spec.Y;
        spec.GenDeploy.X += center.x / 2f;
        spec.GenDeploy.Z += center.y / 2f;
        Rotation rot;
        if (center.y == center.x)
        { // Rotate randomly
            rot = spec.Random.NextRotation();
        }
        else if (center.y > center.x)
        {
            rot = spec.Random.NextBool() ? Rotation.ClockWise : Rotation.CounterClockWise;
        }
        else
        {
            rot = spec.Random.NextBool() ? Rotation.OneEighty : Rotation.None;
        }
        spec.GenDeploy.Rotate(rot);
    }

    protected void PlaceFlush(ITransform deploy, GridLocation loc, float buffer = 0F, bool rough = false)
    {
        switch (loc)
        {
            case GridLocation.TOP:
                deploy.Rotation = 180;
                deploy.X = -this.Bounds.center.x;
                deploy.Z = GetInside(Axis.Z, deploy.Rotation, buffer, rough);
                break;
            case GridLocation.BOTTOM:
                deploy.X = -this.Bounds.center.x;
                deploy.Z = -GetInside(Axis.Z, deploy.Rotation, buffer, rough);
                break;
            case GridLocation.LEFT:
                deploy.Rotation = 90;
                deploy.X = -GetInside(Axis.X, deploy.Rotation, buffer, rough);
                deploy.Z = -this.Bounds.center.z;
                break;
            case GridLocation.RIGHT:
                deploy.Rotation = -90;
                deploy.X = GetInside(Axis.X, deploy.Rotation, buffer, rough);
                deploy.Z = -this.Bounds.center.z;
                break;
        }
    }

    protected void PlaceRandomlyInside(System.Random random, ITransform deploy, float buffer = 0F)
    {
        deploy.Rotation = random.NextAngle();
        deploy.X = RandomInside(random, Axis.X, deploy.Rotation, buffer);
        deploy.Z = RandomInside(random, Axis.Z, deploy.Rotation, buffer);
    }

    protected float RandomInside(System.Random random, Axis axis, float yRotation, float buffer = 0F, bool rough = true)
    {
        return GetInside(axis, yRotation, buffer, rough) * random.NextNegative() * random.NextFloat();
    }

    protected float GetInside(Axis axis, float yRotation, float buffer = 0F, bool rough = true)
    {
        if (rough)
        {
            return .5F - buffer;
        }
        else
        {
            yRotation += this.GO.transform.rotation.y;
            double radians = yRotation * Math.PI / 180d;
            float axisValue;
            switch (axis)
            {
                case Axis.X:
                    axisValue = (float)(Math.Abs(this.Bounds.extents.x * Math.Cos(radians)) + Math.Abs(this.Bounds.extents.z * Math.Sin(radians)));
                    break;
                case Axis.Y:
                    axisValue = this.Bounds.size.y;
                    break;
                default:
                    axisValue = (float)(Math.Abs(this.Bounds.extents.x * Math.Sin(radians)) + Math.Abs(this.Bounds.extents.z * Math.Cos(radians)));
                    break;
            }
            float remaining = 0.5F - axisValue - buffer;
            if (remaining < 0F)
            {
                return 0;
            }
            return remaining;
        }
    }

    protected void SetChar(char c)
    {
        PrintChar = c.ToString();
    }
}

