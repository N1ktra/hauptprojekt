using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Path_Enemy_Manager : MonoBehaviour
{

    public GameObject Enemy;
    public GameObject Player;
    public Pathfinding pathfinding;

    private void Awake()
    {
        pathfinding = GetComponent<Pathfinding>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        pathfinding.AStar(Enemy.transform.position, Player.transform.position);
    }
}
