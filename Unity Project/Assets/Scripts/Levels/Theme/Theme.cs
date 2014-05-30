using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using LevelGen;

public abstract class Theme : ThemeOption, IInitializable
{
    #region Core Elements
    public ThemeElementBundle Wall;
    public ThemeElementBundle Door;
    public ThemeElementBundle Floor;
    public ThemeElementBundle Stair;
    public ThemeElementBundle Chest;
    #endregion
    public RoomModCollection RoomMods { get; protected set; }
    public SpawnKeywords[] Keywords;
    public GenericFlags<SpawnKeywords> KeywordFlags;
    public double AverageRoomRadius;
    public override double AvgRoomRadius
    {
        get
        {
            if (AverageRoomRadius == 0)
            {
                throw new ArgumentException(this + ": Average room radius == 0.  Did you forget to set it?");
            }
            return AverageRoomRadius;
        }
    }

    public virtual void Init()
    {
        RoomMods = new RoomModCollection();
        KeywordFlags = new GenericFlags<SpawnKeywords>(Keywords);
    }

    public override Theme GetTheme(System.Random rand)
    {
        return this;
    }

    public void ChooseAllSmartObjects(System.Random rand)
    {
        foreach (ThemeElementBundle bundle in this.FindAllDerivedObjects<ThemeElementBundle>(false))
        {
            bundle.Select(rand);
        }
    }

    #region Door Placement
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
    #endregion

    public abstract bool GenerateRoom(LevelGenerator gen, Area a, LayoutObject<GenSpace> room);
    
    protected LayoutObject<GenSpace> ModRoom(LevelGenerator gen, Area a, LayoutObject<GenSpace> room)
    {
        #region DEBUG
        double time = 0;
        if (BigBoss.Debug.logging(Logs.LevelGenMain))
        {
            BigBoss.Debug.w(Logs.LevelGenMain, "Mods for " + room);
        }
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Modding " + room);
            time = Time.realtimeSinceStartup;
        }
        #endregion
        this.ChooseAllSmartObjects(gen.Rand);
        RoomSpec spec = new RoomSpec(room, gen.Depth, this, gen.Rand);
        // Base Mod
        if (!ApplyMod(spec, spec.RoomModifiers.BaseMods))
        {
            throw new ArgumentException("Could not apply base mod");
        }
        // Definining Mod
        if (spec.RoomModifiers.AllowDefiningMod)
        {
            ApplyMod(spec, spec.RoomModifiers.DefiningMods);
        }
        // Flex Mods
        int numFlex = gen.Rand.Next(spec.RoomModifiers.MinFlexMods, spec.RoomModifiers.MaxFlexMods);
        int numHeavy = (int)Math.Round((numFlex / 3d) + (numFlex / 3d * gen.Rand.NextDouble()));
        int numFill = numFlex - numHeavy;
        // Heavy Mods
        for (int i = 0; i < numHeavy; i++)
        {
            if (!ApplyMod(spec, spec.RoomModifiers.HeavyMods))
            {
                break;
            }
        }
        // Fill Mods
        for (int i = 0; i < numFill; i++)
        {
            if (!ApplyMod(spec, spec.RoomModifiers.FillMods))
            {
                break;
            }
        }
        // Final Mods
        ApplyMod(spec, spec.RoomModifiers.FinalMods);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            room.ToLog(Logs.LevelGen);
            BigBoss.Debug.w(Logs.LevelGen, "Modding " + room + " took " + (Time.realtimeSinceStartup - time) + " seconds.");
            BigBoss.Debug.printFooter(Logs.LevelGen, "Modding " + room);
        }
        #endregion
        if (!ValidateRoom(room))
        {
            throw new ArgumentException(room + " is not valid.");
        }
        return room;
    }

    protected bool ApplyMod<T>(RoomSpec spec, ProbabilityPool<T> mods)
        where T : RoomModifier
    {
        mods.BeginTaking();
        T mod;
        while (mods.Take(spec.Random, out mod))
        {
            #region DEBUG
            float stepTime = 0;
            if (BigBoss.Debug.logging(Logs.LevelGenMain))
            {
                BigBoss.Debug.w(Logs.LevelGenMain, "   Applying: " + mod);
            }
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                stepTime = Time.realtimeSinceStartup;
                BigBoss.Debug.w(Logs.LevelGen, "Applying: " + mod);
            }
            #endregion
            Container2D<GenSpace> backupGrid = new MultiMap<GenSpace>(spec.Grids);
            if (mod.Modify(spec))
            {
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    spec.Grids.ToLog(Logs.LevelGen, "Applying " + mod + " took " + (Time.realtimeSinceStartup - stepTime) + " seconds.");
                }
                #endregion
                mods.EndTaking();
                return true;
            }
            else
            {
                spec.Grids.Clear();
                spec.Grids.PutAll(backupGrid);
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    spec.Grids.ToLog(Logs.LevelGen, "Couldn't apply mod.  Processing " + mod + " took " + (Time.realtimeSinceStartup - stepTime) + " seconds.");
                }
                #endregion
            }
        }
        mods.EndTaking();
        return false;
    }

    protected bool ValidateRoom(LayoutObject<GenSpace> room)
    {
        return room.Bounding.IsValid();
    }
}
