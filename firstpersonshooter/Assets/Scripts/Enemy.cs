using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class Enemy : MonoBehaviour
{
    private Camera cam;
    private Pathfinding pathfinding;
    public List<Node> path;

    [Header("Stats")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    [Header("Visuals")]
    [SerializeField] private Slider healthBar;


    public void Start()
    {

        cam = Camera.main;
        pathfinding = GetComponent<Pathfinding>();
        path = new List<Node>();
        healthBar.minValue = 0;
        healthBar.maxValue = maxHealth;
        currentHealth = maxHealth;
        UpdateHealthBar();
        StartCoroutine(Repeater());
    }

    private IEnumerator Repeater()
    {
        while (true)
        {

            yield return new WaitForSeconds(3);
            move();
        }
    }


    public void Update()
    {
        healthBar.transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
        if (path.Count==0) // || getDistanceBetween2Vectors(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) > 30
        {
            Debug.Log("test");
            path = pathfinding.AStar(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
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
            transform.position += vectorFromTo(transform.position, addY(next.worldPosition));
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

    /// <summary>
    /// Fügt dem Gegner Schaden zu
    /// </summary>
    /// <param name="amount">Menge an Schaden</param>
    public void takeDamage(float amount)
    {
        currentHealth -= amount;
        UpdateHealthBar();
        if(currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void UpdateHealthBar()
    {
        healthBar.value = currentHealth;
    }
}
