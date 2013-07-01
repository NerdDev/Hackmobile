using System.Collections;
using System.Collections.Generic;
using System;

public class LevelLayout : LayoutObjectContainer {

    List<Room> rooms = new List<Room>();
	List<Path> paths = new List<Path>();

    public void AddRoom(Room r)
    {
        rooms.Add(r);
    }

    public void AddRooms(List<Room> rs)
    {
        rooms.AddRange(rs);
    }
	
	public List<Room> GetRooms()
	{
		return new List<Room>(rooms);
	}

    public void AddPath(Path p)
    {
        paths.Add(p);
    }
	
	public List<Path> GetPaths()
	{
		List<Path> ret = new List<Path>(paths);
		return ret;
	}

    public override IEnumerator<LayoutObject> GetEnumerator()
    {
        foreach (Room room in rooms)
        {
            yield return room;
        }
        foreach (Path path in paths)
        {
            yield return path;
        }
    }
	
	public override string ToString ()
	{
		 return "LevelLayout";
	}
}
