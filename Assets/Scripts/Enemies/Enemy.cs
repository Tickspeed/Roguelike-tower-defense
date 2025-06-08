using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public EnemyStats enemyStats;

    private float currentHealth;

    private float currentSpeed;
    
    public UnityEvent<float, float> OnHealthChanged; // currentHealth, maxHealth
    public UnityEvent OnDeath;

    void Awake()
    {
        if (enemyStats == null)
        {
            Debug.LogError("EnemyStats not assigned for " + gameObject.name);
            enabled = false; // Disable the enemy if no stats are assigned
            return;
        }

        // Initialize runtime stats from the ScriptableObject
        currentHealth = enemyStats.maxHealth;
        currentSpeed = enemyStats.movementSpeed;

        // Invoke health changed event for UI updates (e.g., health bar)
        OnHealthChanged?.Invoke(currentHealth, enemyStats.maxHealth);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
