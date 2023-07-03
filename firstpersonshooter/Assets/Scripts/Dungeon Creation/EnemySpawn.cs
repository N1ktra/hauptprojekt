using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawn : MonoBehaviour
{
    private BSP_Manager bsp;
    private BinaryRoom dungeon;
    private PlayerStats player;

    [Header("Enemy Prefabs")]
    public GameObject Goblin;
    public GameObject Skeleton;
    public GameObject Crusader;
    public GameObject Monster;
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
        player = bsp.player.GetComponent<PlayerStats>();

        spawnEnemyGroup(Goblin, 5, 1);
        spawnEnemyGroup(Skeleton, 3, .35f);
        spawnEnemyGroup(Crusader, 2, .35f);
        spawnEnemyGroup(Monster, 1, .2f);

        spawnBoss();
    }

    private void spawnEnemyGroup(GameObject enemy, int maxPackSize, float spawnChance)
    {
        foreach (BinaryRoom room in dungeon.allRooms)
        {
            if (room == bsp.startRoom) continue;
            Vector3 coords = getRandomPositionInRoom(room, enemy.GetComponent<CapsuleCollider>().radius);
            for (int i = 0; i <= Mathf.Min(maxPackSize, Random.Range(0, maxPackSize + room.distanceToStartRoom)); i++)
            {
                Debug.Log(enemy.name + ": " + (spawnChance + (room.distanceToStartRoom / 20f)));
                if (Random.value <= spawnChance + (room.distanceToStartRoom / 20))
                {
                    GameObject enemyObj = room.spawnObject(enemy, room.RoomContainer, applyRandomOffset(coords));
                    enemyObj.GetComponent<Enemy>().OnEnemyDied += () => player.GetComponent<PlayerStats>().enemiesSlain++;
                }
            }
        }
    }
    private void spawnBoss()
    {
        BinaryRoom room = bsp.endRoom;
        GameObject BossObj = room.spawnObject(Golem, room.RoomContainer, room.coords.getCenterPosition());
        BossObj.GetComponent<Enemy>().OnEnemyDied += () => SceneManager.LoadScene("Menu");
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
