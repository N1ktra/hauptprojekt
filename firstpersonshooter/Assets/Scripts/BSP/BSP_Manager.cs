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
    public int amountOfSplits;

    [Header("Rooms")]
    public int minWidth = 10;
    public int maxWidth = 25;
    public int minHeight = 10;
    public int maxHeight = 25;
    public Vector4 trimTiles = Vector4.one;
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
            ((int)trimTiles.w, (int)trimTiles.x, (int)trimTiles.y, (int)trimTiles.z), 
            maxTrimTiles, 
            corridorMargin, maxCorridorThickness
        );
        BinaryRoom dungeon = new BinaryRoom(new RoomCoords(width, height), design);
        Split(amountOfSplits, dungeon);
        dungeon.Trim();
        dungeon.createNeighborList();
        (BinaryRoom startRoom, BinaryRoom endRoom) = dungeon.AddCorridors();
        dungeon.Instantiate();
        dungeon.addObject(playerPrefab, startRoom.coords.toWorldPosition(design.tileSize) + Vector3.up * 3);
        return dungeon;
    }

    private void Split(int i, BinaryRoom room)
    {
        if (i <= 0) return;

        room.Split();
        if (room.leftRoom != null)
            Split(i - 1, room.leftRoom);

        if (room.rightRoom != null)
            Split(i - 1, room.rightRoom);
    }
}
