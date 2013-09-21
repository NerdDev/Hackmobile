using System.Collections;
using System.Collections.Generic;
using System;

public class LevelLayout : LayoutObjectContainer {

    List<Room> rooms = new List<Room>();
	List<Path> paths = new List<Path>();

    public void AddRoom(Room r)
    {
        rooms.Add(r);
        AddObject(r);
    }

    public void AddRooms(List<Room> rs)
    {
        rooms.AddRange(rs);
    }
	
	public List<Room> GetRooms()
	{
		return new List<Room>(rooms);
	}

    public Room GetRandomRoom()
    {
        return rooms[Probability.LevelRand.Next(rooms.Count)];
    }

    public void AddPath(Path p)
    {
        paths.Add(p);
        AddObject(p, 0);
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
