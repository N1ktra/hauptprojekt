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

    [Header("Stats")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    [Header("Visuals")]
    [SerializeField] private Slider healthBar;

    [Header("Drops")]
    public List<float> dropChances = new List<float>();
    public List<Drop> items = new List<Drop>();

    public virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public virtual void Start()
    {
        cam = Camera.main;
        healthBar.minValue = 0;
        healthBar.maxValue = maxHealth;
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public virtual void Update()
    {
        healthBar.transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
    }

    private void UpdateHealthBar()
    {
        healthBar.value = currentHealth;
    }
    /// <summary>
    /// F�gt dem Gegner Schaden zu
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
                Drop drop = GameObject.Instantiate(items[Random.Range(0, items.Count)], transform.position, Quaternion.identity);
                Debug.Log(gameObject.name + " dropped: " + drop.name);
                return;
            }
        }
    }

}
