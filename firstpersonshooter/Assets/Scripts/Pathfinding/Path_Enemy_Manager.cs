using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Path_Enemy_Manager : MonoBehaviour
{

    public GameObject Enemy;
    public GameObject Player;
    public TEST_Pathfinding pathfinding;

    public bool check;
    private List<Node> path;
    
    private void Awake()
    {
        pathfinding = GetComponent<TEST_Pathfinding>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (check)
        {
            path = pathfinding.AStar(Enemy.transform.position, Player.transform.position);
            check = false;
            new WaitForSeconds(3);
            Debug.Log("set check true");
            //check = true;
        }
    }
    
    /*
    void Start()
    {
        pathfinding = GetComponent<Pathfinding>();
        StartCoroutine(waiter());
    }

    IEnumerator waiter()
    {
        //Wait for 4 seconds
        yield return new WaitForSeconds(4);
        Debug.Log("again");
        pathfinding.AStar(Enemy.transform.position, Player.transform.position);

    }
    */

    /*
     * für jeden Enemy soll zu Beginn einmal AStar aufgerufen werden, dann soll der Enemy sich langsam auf den Spieler hinzubewegen
     * vlt 1 Node pro Sekunde
     * alle 10 Sekunden kann der path aktualisiert werden (durch aufrufen von AStar)
     */
    
}
