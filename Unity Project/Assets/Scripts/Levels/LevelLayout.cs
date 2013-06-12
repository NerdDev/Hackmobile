using System.Collections;
using System.Collections.Generic;
using System;

public class LevelLayout : LayoutObjectContainer {

    List<Room> rooms = new List<Room>();

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

    public override IEnumerator<LayoutObject> GetEnumerator()
    {
        foreach (Room room in rooms)
        {
            yield return room;
        }
    }
	
	public override string ToString ()
	{
		 return "LevelLayout";
	}
}
