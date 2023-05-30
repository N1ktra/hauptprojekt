using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor : Room
{
    public Room leftRoom;
    public Room rightRoom;

    //public Corridor(Room leftRoom, Room rightRoom, GameObject tilePrefab) : 
    //{
    //    this.leftRoom = leftRoom;
    //    this.rightRoom = rightRoom;
    //}

    public Corridor(Coords coords, GameObject tilePrefab) : base(coords, tilePrefab) { }
}
