using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BSP_Manager : MonoBehaviour
{
    public GameObject player;
    public BinaryRoom dungeon { get; private set; }
    public event Action<BinaryRoom> OnDungeonCreated;

    public bool DisableDistantRooms = true;
    public Room currentRoom;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject corridorEntrancePrefab;
    public GameObject torchPrefab;
    public GameObject pillarPrefab;

    [Header("Dungeon Size")]
    public int width = 100;
    public int height = 100;
    public int amountOfSplits = 3;
    public Vector3 tileSize = Vector3.one;

    [Header("Dungeon Layout")]
    public int torchPadding = 3;
    public int pillarPadding = 4;

    [Header("Room Size")]
    public int minWidth = 10;
    public int maxWidth = 25;
    public int minHeight = 10;
    public int maxHeight = 25;
    public int wallHeight = 2;

    [Header("Room Trimming")]
    public bool trimTilesIsRandom = true;
    public Vector4 trimTiles = Vector4.one;
    public int minTrimTiles = 0;
    public int maxTrimTiles = 5;

    [Header("Corridoros")]
    public int corridorMargin = 1;
    public int maxCorridorThickness = 5;

    // Start is called before the first frame update
    void Start()
    {
        dungeon = CreateDungeon();
        OnDungeonCreated?.Invoke(dungeon);
    }

    private void Update()
    {
        if (DisableDistantRooms && dungeon != null)
            disableDistantRooms();
    }

    public BinaryRoom CreateDungeon()
    {
        tileSize = new Vector3(floorPrefab.GetComponent<Renderer>().bounds.size.x, wallPrefab.GetComponentInChildren<Renderer>().bounds.size.y, floorPrefab.GetComponent<Renderer>().bounds.size.z);
        RoomDesign design = new RoomDesign(
            tileSize,
            floorPrefab, wallPrefab, corridorEntrancePrefab, torchPrefab, pillarPrefab,
            torchPadding, pillarPadding,
            minWidth, maxWidth, minHeight, maxHeight, wallHeight,
            trimTilesIsRandom,
            ((int)trimTiles.x, (int)trimTiles.y, (int)trimTiles.z, (int)trimTiles.w), 
            minTrimTiles, maxTrimTiles, 
            corridorMargin, maxCorridorThickness
        );
        BinaryRoom dungeon = new BinaryRoom(new RoomCoords(width, height), design);
        GameObject dungeonContainer = new GameObject("Dungeon");
        SplitDungeon(amountOfSplits, dungeon);
        dungeon.Trim();
        dungeon.createNeighborList();
        (BinaryRoom startRoom, BinaryRoom endRoom) = dungeon.AddCorridors();
        if (startRoom == null || endRoom == null)
            return null;
        dungeon.Instantiate();
        foreach(BinaryRoom room in dungeon.allRooms)
        {
            room.RoomContainer.transform.SetParent(dungeonContainer.transform, true);
            foreach(Corridor corridor in room.corridors)
            {
                corridor.RoomContainer.transform.SetParent(dungeonContainer.transform, true);
            }
        }
        player = dungeon.spawnObject(playerPrefab, null, startRoom.coords.getCenterPosition());
        return dungeon;
    }

    private void SplitDungeon(int i, BinaryRoom room)
    {
        if (i <= 0) return;

        room.Split();
        if (room.leftRoom != null)
            SplitDungeon(i - 1, room.leftRoom);

        if (room.rightRoom != null)
            SplitDungeon(i - 1, room.rightRoom);
    }

    private void disableDistantRooms()
    {
        Vector3 playerPos = dungeon.getPositionInRoomCoords(player.transform.position);
        Room newCurrentRoom = null;
        foreach(BinaryRoom room in dungeon.allRooms)
        {
            if (room.coords.Contains(playerPos, 1))
            {
                newCurrentRoom = room;
            }
            else if (room != currentRoom && !room.isConnectedTo(currentRoom))
            {
                room.SetActive(false);
            }
        }
        if (newCurrentRoom != null && newCurrentRoom != currentRoom)
        {
            Debug.Log("room changed");
            currentRoom = newCurrentRoom;
            newCurrentRoom.SetActive(true);
        }
    }

    public bool PlayerIsInRoom(Room room)
    {
        Vector3 playerPos = dungeon.getPositionInRoomCoords(player.transform.position);
        return room.coords.Contains(playerPos, 1);
    }
    public Room getRoomOf(GameObject obj)
    {
        Vector3 objPosition = dungeon.getPositionInRoomCoords(obj.transform.position);
        foreach (BinaryRoom room in dungeon.allRooms)
        {
            if (room.coords.Contains(objPosition))
                return room;
            foreach(Corridor corridor in room.corridors)
            {
                if(corridor.coords.Contains(objPosition, 1))
                    return corridor;
            }
        }
        return null;
    }
}
