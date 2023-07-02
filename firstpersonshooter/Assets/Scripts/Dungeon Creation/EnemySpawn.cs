using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    private BSP_Manager bsp;
    private BinaryRoom dungeon;

    [Header("Enemy Prefabs")]
    public GameObject Goblin;
    public GameObject Skeleton;
    public GameObject Crusader;
    public GameObject Golem;

    // Start is called before the first frame update
    void Awake()
    {
        bsp = GetComponent<BSP_Manager>();
        bsp.OnDungeonCreated += SpawnEnemies;
    }

    public void SpawnEnemies(BinaryRoom dungeon)
    {
        this.dungeon = dungeon;
        spawnEnemyGroup(Goblin, 5, 1);
        spawnEnemyGroup(Skeleton, 3, .5f);
        spawnEnemyGroup(Crusader, 1, .2f);

        spawnBoss();
    }

    private void spawnEnemyGroup(GameObject enemy, int maxPackSize, float spawnChance)
    {
        foreach (Room room in dungeon.allRooms)
        {
            Vector3 coords = getRandomPositionInRoom(room, enemy.GetComponent<CapsuleCollider>().radius);
            for (int i = 0; i <= Random.Range(0, maxPackSize); i++)
            {
                if (Random.value <= spawnChance)
                    room.spawnObject(enemy, room.RoomContainer, applyRandomOffset(coords));
            }
        }
    }
    private void spawnBoss()
    {
        BinaryRoom room = bsp.endRoom;
        room.spawnObject(Golem
            
            
            
            
            
   , room.RoomContainer, room.coords.getCenterPosition());
    }

    private Vector3 getRandomPositionInRoom(Room room, float checkRadius)
    {
        Vector3 coords = room.coords.getRandomPosition();
        for (int j = 0; j < 100; j++)
        {
            if (!room.checkCollision(coords, checkRadius))
                return coords;
            else
                coords = room.coords.getRandomPosition();
        }
        Debug.Log("couldn't find random position in room");
        return coords;
    }

    public static Vector3 applyRandomOffset(Vector3 coords, float minOffset = 0, float maxOffset = 1)
    {
        coords += new Vector3(Random.Range(minOffset, maxOffset), 0, Random.Range(minOffset, maxOffset));
        return coords;
    }

}
