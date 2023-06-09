using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum EnemyState
{
    IDLE,
    MOVING,
    ATTACKING,
    DYING,
    HIT
}
public abstract class Enemy : MonoBehaviour
{
    public event Action OnEnemyDied;

    [Header("References")]
    protected Camera cam;
    protected GameObject player;
    protected Pathfinding pathfinding;
    protected BSP_Manager bsp_manager;
    protected Animator animator;

    [Header("Behavior")]
    public EnemyState state;

    [Header("Stats")]
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float currentHealth;
    [SerializeField] protected float attackDamage;

    [Header("Visuals")]
    [SerializeField] private Slider healthBar;

    [Header("Drops")]
    public List<float> dropChances = new List<float>();
    public List<Drop> dropItems = new List<Drop>();

    [Header("A*")]
    public List<Node> path = new List<Node>();


    public virtual void Awake()
    {
        cam = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player");
        pathfinding = GameObject.Find("PathfindingObject").GetComponent<Pathfinding>();
        bsp_manager = GameObject.Find("Dungeon Manager").GetComponent<BSP_Manager>();
        animator = GetComponent<Animator>();
    }

    public virtual void Start()
    {
        healthBar.minValue = 0;
        healthBar.maxValue = maxHealth;
        currentHealth = maxHealth;
        UpdateHealthBar();
        state = EnemyState.IDLE;
    }

    public virtual void Update()
    {
        if(healthBar != null)
            healthBar.transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
    }

    public void OnEnable()
    {
        StartBehavior();
    }

    private Coroutine behavior;
    protected event Action OnBehaviorStarted;
    public void StartBehavior()
    {
        if(behavior != null)
        {
            StopCoroutine(behavior);
        }
        behavior = StartCoroutine(Behavior());
        OnBehaviorStarted?.Invoke();
    }

    public abstract IEnumerator Behavior();

    public void ChangeState(EnemyState state)
    {
        if (this.state == state && animator.GetInteger("State") == (int)state) return;
        this.state = state;
        animator.StopPlayback();
        animator.SetInteger("State", (int)state);
    }

    public void CalculatePath()
    {
        //Debug.Log("WEG BERECHNET");
        //Debug.Log("GegnerPosition: " + this.transform.position + " , PlayerPosition: " + player.transform.position);
        path = pathfinding.AStar(this.transform.position, player.transform.position);
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
        if (state == EnemyState.DYING) return;
        ChangeState(EnemyState.HIT);
        StartBehavior();
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
        healthBar.gameObject.SetActive(false);
        ChangeState(EnemyState.DYING);
        OnEnemyDied?.Invoke();
        Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
    }

    public void dropItem()
    {
        if(dropChances.Count != dropItems.Count)
        {
            Debug.LogWarning("Number of items doesnt match number of dropChances.");
            return;
        }
        for(int i = 0; i < dropItems.Count; i++)
        {
            float value = Random.value;
            if (value <= dropChances[i])
            {
                Drop drop = GameObject.Instantiate(dropItems[i], transform.position, Quaternion.identity);
                Debug.Log(gameObject.name + " dropped: " + drop.name);
                return;
            }
        }
    }

}
