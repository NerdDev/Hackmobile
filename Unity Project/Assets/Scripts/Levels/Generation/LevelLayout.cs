using System.Collections;
using System.Collections.Generic;
using System;

public class LevelLayout : LayoutObjectContainer
{
    public Point UpStart;
    public Point DownStart;
    List<LayoutObjectLeaf> rooms = new List<LayoutObjectLeaf>();
    List<Path> paths = new List<Path>();

    public void AddRoom(LayoutObjectLeaf r)
    {
        rooms.Add(r);
        AddObject(r);
    }

    public void AddRooms(List<LayoutObjectLeaf> rs)
    {
        rooms.AddRange(rs);
    }

    public List<LayoutObjectLeaf> GetRooms()
    {
        return new List<LayoutObjectLeaf>(rooms);
    }

    public LayoutObjectLeaf GetRandomRoom(System.Random rand)
    {
        return rooms[rand.Next(rooms.Count)];
    }

    public void AddPath(Path p)
    {
        paths.Add(p);
        AddObject(p);
    }

    public List<Path> GetPaths()
    {
        List<Path> ret = new List<Path>(paths);
        return ret;
    }

    public override string GetTypeString()
    {
        return "Level Layout";
    }
}
