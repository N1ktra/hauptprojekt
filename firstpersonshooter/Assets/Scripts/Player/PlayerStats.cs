using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth { get; private set; }
    public float shield;
    public float stamina;

    public float enemiesSlain;

    public void Start()
    {
        currentHealth = maxHealth;
        enemiesSlain = 0;
    }

    public void takeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            //end game
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("Menu");
            //Destroy(gameObject);
        }
    }

    public void addHealth(float amount)
    {
        currentHealth += Mathf.Min(amount, maxHealth - currentHealth);
    }
}
