using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    private BSP_Manager bsp;

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
            GameObject enemy = room.spawnObject(standardEnemy, room.RoomContainer, room.coords.getRandomPosition());
        }
    }

}
