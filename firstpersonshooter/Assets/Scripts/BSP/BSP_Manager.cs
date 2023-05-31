using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BSP_Manager : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        BinaryRoom dungeon = CreateDungeon();
    }

    public BinaryRoom CreateDungeon()
    {
        BinaryRoom dungeon = new BinaryRoom(new Room.Coords(100, 100), floorPrefab.GetComponent<Renderer>().bounds.size);
        Split(3, dungeon);
        dungeon.Trim();
        dungeon.createNeighborList();
        (BinaryRoom startRoom, BinaryRoom endRoom) = dungeon.AddCorridors();
        dungeon.Instantiate(floorPrefab, wallPrefab);
        dungeon.addObject(playerPrefab, startRoom.coords.toWorldPosition(startRoom.tileSize) + Vector3.up * 3);
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
