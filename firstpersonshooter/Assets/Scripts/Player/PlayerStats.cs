using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth { get; private set; }
    public float shield;
    public float stamina;

    public void Start()
    {
        currentHealth = maxHealth;
    }

    public void takeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            //TODO: Spiel beenden
            Destroy(gameObject);
        }
    }

    public void addHealth(float amount)
    {
        currentHealth += Mathf.Min(amount, maxHealth - currentHealth);
    }
}
