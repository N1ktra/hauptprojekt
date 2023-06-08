using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    private BSP_Manager bsp;

    public int maxEnemyPackSize = 3;

    [Header("Enemy Prefabs")]
    public GameObject standardEnemy;

    // Start is called before the first frame update
    void Awake()
    {
        bsp = GetComponent<BSP_Manager>();
        bsp.OnDungeonCreated += SpawnEnemies;
    }

    public void SpawnEnemies(BinaryRoom dungeon)
    {
        foreach (Room room in dungeon.allRooms)
        {
            //GridManager grid = room.RoomContainer.AddComponent<GridManager>();
            //grid.Init(new Vector2(room.coords.GetWidth() * room.design.tileSize.x, room.coords.GetHeight() * room.design.tileSize.z), 1, LayerMask.GetMask("unwalkable"), bsp.player.transform);
            for(int i = 0; i <= Random.Range(0, maxEnemyPackSize); i++)
            {
                GameObject enemy = room.spawnObject(standardEnemy, room.RoomContainer, getRandomPositionInRoom(room));
            }
        }
    }

    private Vector3 getRandomPositionInRoom(Room room)
    {
        Vector3 coords = room.coords.getRandomPosition();
        for (int j = 0; j < 100; j++)
        {
            if (!Physics.CheckSphere(room.getPositionInWorldCoords(coords) + Vector3.up, standardEnemy.GetComponent<CapsuleCollider>().radius))
                return coords;
            else
                coords = room.coords.getRandomPosition();
        }
        Debug.Log("couldn't find random position in room");
        return coords;
    }

}
