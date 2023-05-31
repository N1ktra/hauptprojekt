using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BSP_Manager : MonoBehaviour
{
    public GameObject TilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        BinaryRoom dungeon = CreateDungeon();
    }

    public BinaryRoom CreateDungeon()
    {
        BinaryRoom dungeon = new BinaryRoom(new Room.Coords(100, 100), TilePrefab);
        Split(5, dungeon);
        dungeon.Trim();
        dungeon.createNeighborList();
        dungeon.AddCorridors();
        dungeon.Instantiate();
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
