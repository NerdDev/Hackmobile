using System.Collections;
using System.Collections.Generic;
using System;

public class LevelLayout : LayoutObjectContainer {

    List<Room> rooms = new List<Room>();
	List<Path> paths = new List<Path>();

    public void addRoom(Room r)
    {
        rooms.Add(r);
    }

    public void addRooms(List<Room> rs)
    {
        rooms.AddRange(rs);
    }
	
	public List<Room> getRooms()
	{
		List<Room> ret = new List<Room>(rooms);
		return ret;
	}

    public void addPath(Path p)
    {
        paths.Add(p);
    }
	
	public List<Path> getPaths()
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
