using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    protected Camera cam;
    protected GameObject player;
    protected Pathfinding pathfinding;
    protected BSP_Manager bsp_manager;

    [Header("Stats")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    [Header("Visuals")]
    [SerializeField] private Slider healthBar;

    [Header("Drops")]
    public List<float> dropChances = new List<float>();
    public List<Drop> items = new List<Drop>();

    [Header("A*")]
    public List<Node> path = new List<Node>();
    

    public virtual void Awake()
    {
        cam = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player");
        pathfinding = GameObject.Find("PathfindingObject").GetComponent<Pathfinding>();
        bsp_manager = GameObject.Find("Dungeon Manager").GetComponent<BSP_Manager>();
    }

    public virtual void Start()
    {
        healthBar.minValue = 0;
        healthBar.maxValue = maxHealth;
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public virtual void Update()
    {
        healthBar.transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position); 
        if (path.Count == 0) // || getDistanceBetween2Vectors(player.transform.position, transform.position) > 30
        {
            path = pathfinding.AStar(transform.position, player.transform.position);
            Debug.Log("WEG BERECHNET");
        }
    }

    private void UpdateHealthBar()
    {
        healthBar.value = currentHealth;
    }
    /// <summary>
    /// Fügt dem Gegner Schaden zu
    /// </summary>
    /// <param name="amount">Menge an Schaden</param>
    public virtual void takeDamage(float amount)
    {
        currentHealth -= amount;
        UpdateHealthBar();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        dropItem();
        Destroy(gameObject);
    }

    public void dropItem()
    {
        if(dropChances.Count != items.Count)
        {
            Debug.LogWarning("Number of items doesnt match number of dropChances.");
            return;
        }
        for(int i = 0; i < items.Count; i++)
        {
            float value = Random.value;
            if (value <= dropChances[i])
            {
                Drop drop = GameObject.Instantiate(items[i], transform.position, Quaternion.identity);
                Debug.Log(gameObject.name + " dropped: " + drop.name);
                return;
            }
        }
    }

}
