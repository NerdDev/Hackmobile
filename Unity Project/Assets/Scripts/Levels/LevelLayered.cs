using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelLayered : Level {

    List<Room> rooms = new List<Room>();

    public LevelLayered () {
    }

    public override MultiMap<GridBox> getFlat()
    {
        return getFlatLevel().getFlat();
    }

    public LevelFlat getFlatLevel()
    {
        LevelFlat ret = new LevelFlat();
        throw new NotImplementedException();
        return ret;
    }

    public void addRoom(Room r)
    {
        rooms.Add(r);
    }

    public void addRooms(List<Room> rs)
    {
        rooms.AddRange(rs);
    }

    public override GridBox get(int x, int y)
    {
        throw new NotImplementedException();
    }
}
