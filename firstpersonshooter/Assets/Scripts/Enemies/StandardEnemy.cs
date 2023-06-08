using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class StandardEnemy : Enemy
{
    private Pathfinding pathfinding;
    public List<Node> path;


    public override void Start()
    {
        base.Start();
        pathfinding = GetComponent<Pathfinding>();
        path = new List<Node>();
        StartCoroutine(Repeater());
    }

    private IEnumerator Repeater()
    {
        while (true)
        {

            yield return new WaitForSeconds(1);
            move();
        }
    }

    public override void Update()
    {
        base.Update();
        if (path.Count==0) // || getDistanceBetween2Vectors(player.transform.position, transform.position) > 30
        {
            path = pathfinding.AStar(transform.position, player.transform.position);
        }
    }

    public List<Node> getPath()
    {
        if(path == null) { return null; }
        else { return path; }
    }

    private float counter;

    //moves one node and deletes it from path List
    private void move()
    {
        if (path.Count >= 1) {
            Node next = path.First();
            path.RemoveAt(0);
            transform.DOMove(addY(next.worldPosition), 1f);
            //transform.position += vectorFromTo(transform.position, addY(next.worldPosition));
            //this.GetComponent<Rigidbody>().transform.position += vectorFromTo(transform.position, addY(next.worldPosition)) *  Time.fixedDeltaTime;
            Debug.Log("bewegt");
        }
    }

    private Vector3 addY(Vector3 pos)
    {
        Vector3 added = new Vector3(pos.x, pos.y + 1, pos.z);
        return added;
    }

    private Vector3 vectorFromTo(Vector3 start, Vector3 end)
    {
        Vector3 way = new Vector3(end.x - start.x, end.y - start.y, end.z - start.z);
        Debug.Log("Way: x: " + way.x + " z: " + way.z);
        return way;
    }


    private int getDistanceBetween2Vectors(Vector3 v1, Vector3 v2)
    {
        return (int) (Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y-v2.y)+Mathf.Abs(v1.z-v2.z));
    }
}