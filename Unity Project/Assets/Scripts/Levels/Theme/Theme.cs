using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Theme : ThemeOption, IInitializable
{
    public bool Scatterable;
    public bool Chainable;
    #region Core Elements
    public ThemeElementBundle Wall;
    public ThemeElementBundle Door;
    public ThemeElementBundle Floor;
    public ThemeElementBundle Stair;
    public ThemeElementBundle Chest;
    #endregion
    protected RoomModCollection _roomMods;
    public SpawnKeywords[] Keywords;
    public GenericFlags<SpawnKeywords> KeywordFlags;

    public virtual void Init()
    {
        _roomMods = new RoomModCollection();
        KeywordFlags = new GenericFlags<SpawnKeywords>(Keywords);
    }

    public RoomModCollection GetRoomMods()
    {
        return new RoomModCollection(_roomMods);
    }

    public void ChooseAllSmartObjects(System.Random rand)
    {
        foreach (ThemeElementBundle bundle in this.FindAllDerivedObjects<ThemeElementBundle>(false))
        {
            bundle.Select(rand);
        }
    }

    public Theme Flatten()
    {
        Theme ret = (Theme) this.MemberwiseClone();
        Type bundleType = typeof(ThemeElementBundle);
        foreach (var field in ret.GetType().GetFields())
        {
            if (bundleType.IsAssignableFrom(bundleType))
            {
                ThemeElementBundle bundle = (ThemeElementBundle) field.GetValue(this);
                field.SetValue(this, bundle.SmartElement);
            }
        }
        return ret;
    }

    public static ProbabilityList<int> DoorRatioPicker;
    public List<Value2D<GenSpace>> PlaceSomeDoors(Container2D<GenSpace> arr, IEnumerable<Point> points, System.Random rand, int desiredWallToDoorRatio = -1, bool expandDoors = true)
    {
        if (desiredWallToDoorRatio < 0)
        {
            desiredWallToDoorRatio = LevelGenerator.desiredWallToDoorRatio;
        }
        var acceptablePoints = new MultiMap<GenSpace>();
        Counter numPoints = new Counter();
        DrawAction<GenSpace> call = Draw.Count<GenSpace>(numPoints).And(Draw.CanDrawDoor().IfThen(Draw.AddTo(acceptablePoints)));
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
        int numDoors = numPoints / desiredWallToDoorRatio;
        numDoors += DoorRatioPicker.Get(rand);
        if (numDoors <= 0)
        {
            numDoors = 1;
        }
        List<Value2D<GenSpace>> pickedPts = acceptablePoints.GetRandom(rand, numDoors, 1);
        DrawAction<GenSpace> additionalTest = null;
        MultiMap<GenSpace> notAllowed = null;
        if (expandDoors)
        {
            MultiMap<GenSpace> allowed = new MultiMap<GenSpace>();
            foreach (Point p in points)
            {
                allowed[p] = null;
            }
            notAllowed = new MultiMap<GenSpace>();
            foreach (var v in pickedPts)
            {
                notAllowed[v] = null;
            }
            additionalTest = Draw.PointContainedIn(allowed).And(Draw.Not(Draw.HasAround(false, Draw.PointContainedIn(notAllowed))));
        }
        foreach (Point picked in pickedPts)
        {
            if (expandDoors)
            {
                notAllowed.Remove(picked);
                PlaceDoor(arr, picked.x, picked.y, rand, additionalTest);
                notAllowed[picked] = null;
            }
            else
            {
                arr.SetTo(picked, GridType.Door, this);
            }
        }
        return pickedPts;
    }

    public void PlaceDoor(Container2D<GenSpace> cont, int x, int y, System.Random rand, DrawAction<GenSpace> additionalTest = null)
    {
        DrawAction<GenSpace> test = Draw.CanDrawDoor();
        if (additionalTest != null)
        {
            test = test.And(additionalTest);
        }

        // Count largest option
        Counter horiz = new Counter();
        cont.DrawLineExpanding(x, y, GridDirection.HORIZ, 5, test.And(Draw.Count<GenSpace>(horiz)));
        Counter vert = new Counter();
        cont.DrawLineExpanding(x, y, GridDirection.VERT, 5, test.And(Draw.Count<GenSpace>(vert)));

        // Find largest
        GridDirection dir;
        MultiMap<GenSpace> map;
        int count;
        if (horiz.Count < vert || (horiz == vert && rand.NextBool()))
        {
            count = vert;
            dir = GridDirection.VERT;
        }
        else
        {
            count = horiz;
            dir = GridDirection.HORIZ;
        }
        count = Math.Max(count, 1);

        // Pick random size
        for (; count > 1; count--)
        {
            if (rand.NextBool())
            {
                break;
            }
        }

        // Pick door
        SmartThemeElement doorElement;
        while (!Door.Select(rand, count, 1, out doorElement) && count > 1)
        {
            count--;
        }

        ThemeElement door = doorElement.Get(rand);
        cont.DrawLineExpanding(x, y, dir, count / 2, Draw.MergeIn(door, this, GridType.Door, false).And(Draw.Around(false, Draw.IsNull<GenSpace>().IfThen(Draw.SetTo(GridType.Floor, this)))));
    }

    public override Theme GetTheme(System.Random rand)
    {
        return this;
    }
}
