using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    private BSP_Manager bsp;

    [Header("Enemy Prefabs")]
    public GameObject standardEnemy;

    // Start is called before the first frame update
    void Start()
    {
        bsp = GetComponent<BSP_Manager>();
        bsp.OnDungeonCreated += SpawnEnemies;
    }

    public void SpawnEnemies(BinaryRoom dungeon)
    {
        foreach(Room room in dungeon.allRooms)
        {
            room.spawnObject(standardEnemy, room.RoomContainer, room.coords.getRandomPosition());
        }
    }

}
