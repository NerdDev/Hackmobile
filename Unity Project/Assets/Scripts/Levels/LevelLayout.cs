using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelLayout {

    List<Room> rooms = new List<Room>();

    public LevelLayout () {
    }

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
		return new List<Room>(rooms);
	}
}
