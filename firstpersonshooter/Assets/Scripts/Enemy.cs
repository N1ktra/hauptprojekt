using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    }

    public void Update()
    {
        healthBar.transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
        if (path.Count<=4) // || getDistanceBetween2Vectors(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) > 30
        {
            Debug.Log("test");
            path = pathfinding.AStar(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
        }
        
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
