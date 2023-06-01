using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BSP_Manager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject playerPrefab;

    [Header("Dungeon")]
    public int width = 100;
    public int height = 100;
    public int amountOfSplits = 3;

    [Header("Room Size")]
    public int minWidth = 10;
    public int maxWidth = 25;
    public int minHeight = 10;
    public int maxHeight = 25;

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
        BinaryRoom dungeon = CreateDungeon();
    }

    public BinaryRoom CreateDungeon()
    {
        RoomDesign design = new RoomDesign(
            floorPrefab.GetComponent<Renderer>().bounds.size, 
            floorPrefab, wallPrefab, 
            minWidth, maxWidth, minHeight, maxHeight,
            trimTilesIsRandom,
            ((int)trimTiles.x, (int)trimTiles.y, (int)trimTiles.z, (int)trimTiles.w), 
            minTrimTiles, maxTrimTiles, 
            corridorMargin, maxCorridorThickness
        );
        BinaryRoom dungeon = new BinaryRoom(new RoomCoords(width, height), design);
        SplitDungeon(amountOfSplits, dungeon);
        dungeon.Trim();
        dungeon.createNeighborList();
        (BinaryRoom startRoom, BinaryRoom endRoom) = dungeon.AddCorridors();
        dungeon.Instantiate();
        dungeon.addObject(playerPrefab, startRoom.coords.toWorldPosition(design.tileSize) + Vector3.up * 3);
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
}
