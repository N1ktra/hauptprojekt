using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private Camera cam;

    [Header("Stats")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    [Header("Visuals")]
    [SerializeField] private Slider healthBar;


    public void Start()
    {
        cam = Camera.main;
        healthBar.minValue = 0;
        healthBar.maxValue = maxHealth;
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void Update()
    {
        healthBar.transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
    }

    /// <summary>
    /// F�gt dem Gegner Schaden zu
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
