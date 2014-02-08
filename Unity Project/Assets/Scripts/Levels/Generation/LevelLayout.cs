using System.Collections;
using System.Collections.Generic;
using System;

public class LevelLayout : LayoutObjectContainer
{
    public Point UpStart;
    public Point DownStart;
    List<LayoutObject> rooms = new List<LayoutObject>();

    public void AddRoom(LayoutObject r)
    {
        rooms.Add(r);
        AddObject(r);
    }

    public void AddRooms(List<LayoutObject> rs)
    {
        rooms.AddRange(rs);
    }

    public List<LayoutObject> GetRooms()
    {
        return new List<LayoutObject>(rooms);
    }

    public LayoutObject GetRandomRoom(System.Random rand)
    {
        return rooms[rand.Next(rooms.Count)];
    }
}
