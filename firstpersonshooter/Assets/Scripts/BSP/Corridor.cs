using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor : Room
{
    public Corridor(RoomCoords coords, RoomDesign design, DIRECTION direction, (BinaryRoom startRoom, BinaryRoom endRoom) rooms) : base(coords, design) 
    {
        this.direction = direction;
        this.rooms = rooms;
    }
    public DIRECTION direction;
    public (BinaryRoom startRoom, BinaryRoom endRoom) rooms;
    private bool isInstantiated = false;

    /// <summary>
    /// setzt start & end room und jeweils deren Corridors
    /// </summary>
    /// <param name="active"></param>
    public override void SetActive(bool active)
    {
        isActive = active;
        if(active)
        {
            rooms.startRoom.RoomContainer.SetActive(active);
            rooms.startRoom.isActive = active;
            foreach(Corridor corridor in rooms.startRoom.corridors)
            {
                corridor.RoomContainer.SetActive(active);
            }
            rooms.endRoom.RoomContainer.SetActive(active);
            rooms.endRoom.isActive = active;
            foreach (Corridor corridor in rooms.endRoom.corridors)
            {
                corridor.RoomContainer.SetActive(active);
            }
            RoomContainer.SetActive(active);
        }
        else
        {
            RoomContainer.SetActive(rooms.startRoom.isActive || rooms.endRoom.isActive);
        }
    }
    public bool Connects(Room room)
    {
        return rooms.startRoom == room || rooms.endRoom == room;
    }

    public override GameObject Instantiate()
    {
        if (isInstantiated)
        {
            //Debug.Log("corridor has already been instantiated");
            return null;
        }
        RoomContainer = new GameObject("Corridor");
        instantiateFloor().transform.SetParent(RoomContainer.transform, true);
        instantiateWalls().transform.SetParent(RoomContainer.transform, true);
        instantiateCeiling().transform.SetParent(RoomContainer.transform, true);
        isInstantiated = true;
        return RoomContainer;
    }

    public GameObject instantiateWalls()
    {
        GameObject wallContainer = new GameObject("Wall");
        if(direction == DIRECTION.TOP || direction == DIRECTION.BOTTOM)
        {
            for (int z = coords.bottom; z <= coords.top; z++)
            {
                for (int y = 0; y < design.wallHeight; y++)
                {
                    addObject(design.wallPrefab, wallContainer, new Vector3(coords.left, y, z), Quaternion.Euler(0, 90, 0));
                    addObject(design.wallPrefab, wallContainer, new Vector3(coords.right, y, z), Quaternion.Euler(0, -90, 0));
                }
            }
            //Entrance
            for (int x = coords.left; x <= coords.right; x++)
            {
                addObject(design.corridorEntrancePrefab, wallContainer, new Vector3(x, 0, coords.bottom));
                addObject(design.corridorEntrancePrefab, wallContainer, new Vector3(x, 0, coords.top), Quaternion.Euler(0, 180, 0));
            }
        }
        if (direction == DIRECTION.LEFT || direction == DIRECTION.RIGHT)
        {
            for (int x = coords.left; x <= coords.right; x++)
            {
                for (int y = 0; y < design.wallHeight; y++)
                {
                    addObject(design.wallPrefab, wallContainer, new Vector3(x, y, coords.bottom));
                    addObject(design.wallPrefab, wallContainer, new Vector3(x, y, coords.top), Quaternion.Euler(0, 180, 0));
                }
            }
            //Entrance
            for (int z = coords.bottom; z <= coords.top; z++)
            {
                addObject(design.corridorEntrancePrefab, wallContainer, new Vector3(coords.left, 0, z), Quaternion.Euler(0, 90, 0));
                addObject(design.corridorEntrancePrefab, wallContainer, new Vector3(coords.right, 0, z), Quaternion.Euler(0, -90, 0));
            }
        }
        return wallContainer;
    }
}
