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
        BinaryRoom room = new BinaryRoom(100, 100, TilePrefab);
        Split(8, room);
        room.Trim();
        room.CreateRoom();
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
