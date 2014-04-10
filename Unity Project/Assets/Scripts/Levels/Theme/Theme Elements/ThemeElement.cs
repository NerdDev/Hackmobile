using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ThemeElement : FOWRenderers
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
    public byte GridLength = 1;
    public bool Walkable;
    public string PrintChar = string.Empty;
    public bool Dynamic;

    public virtual void PreDeployTweaks(ThemeElementSpec spec)
    {
    }

    public MultiMap<List<GenDeploy>> PlaceFloors(ThemeElementSpec spec)
    {
        var ret = new MultiMap<List<GenDeploy>>();
        foreach (var space in spec)
        {
            spec.AddAdditional(spec.Theme.Floor.SmartElement.Get(spec.Random), space.x, space.y);
        }
        return ret;
    }

    protected void CenterAndRotateDoodad(ThemeElementSpec spec)
    {
        HandleDoodad(spec, true, true);
    }

    protected void CenterDoodad(ThemeElementSpec spec)
    {
        HandleDoodad(spec, true, false);
    }

    protected void RotateDoodad(ThemeElementSpec spec)
    {
        HandleDoodad(spec, false, true);
    }

    private void HandleDoodad(ThemeElementSpec spec, bool centerB, bool rotateB)
    {
        Vector2 center = spec.Bounding.GetRealCenter();
        if (centerB
            && (spec.GenDeploy.Element.GridLength > 1
                || spec.GenDeploy.Element.GridWidth > 1))
        {
            center.x -= spec.DeployX;
            center.y -= spec.DeployY;
            spec.GenDeploy.X += center.x;
            spec.GenDeploy.Z += center.y;
        }
        if (rotateB)
        {
            Rotation rot;
            if (spec.GenDeploy.Element.GridLength == spec.GenDeploy.Element.GridWidth)
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
    }

    protected void PlaceFlush(ITransform deploy, GridLocation loc, float buffer = 0F, bool rough = false)
    {
        switch (loc)
        {
            case GridLocation.TOP:
                deploy.YRotation = 180;
                deploy.X = -this.Bounds.center.x;
                deploy.Z = GetInside(Axis.Z, deploy.YRotation, buffer, rough);
                break;
            case GridLocation.BOTTOM:
                deploy.X = -this.Bounds.center.x;
                deploy.Z = -GetInside(Axis.Z, deploy.YRotation, buffer, rough);
                break;
            case GridLocation.LEFT:
                deploy.YRotation = 90;
                deploy.X = -GetInside(Axis.X, deploy.YRotation, buffer, rough);
                deploy.Z = -this.Bounds.center.z;
                break;
            case GridLocation.RIGHT:
                deploy.YRotation = -90;
                deploy.X = GetInside(Axis.X, deploy.YRotation, buffer, rough);
                deploy.Z = -this.Bounds.center.z;
                break;
        }
    }

    protected void PlaceRandomlyInside(System.Random random, ITransform deploy, float buffer = 0F)
    {
        deploy.YRotation = random.NextAngle();
        deploy.X = RandomInside(random, Axis.X, deploy.YRotation, buffer);
        deploy.Z = RandomInside(random, Axis.Z, deploy.YRotation, buffer);
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

