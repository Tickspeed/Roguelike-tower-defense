using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    
    public string enemyName = "New Enemy";
    [Header("Attack Stats")]
    public float damage = 10f;
    public float attackSpeed = 100f;

    [Header("Defensive Stats")]
    public float maxHealth = 100f;
    public float physicalDefense = 100f;
    public float magicalDefense = 10f;

    [Header("Other Stats")]
    public float movementSpeed = 100f;
    public int damageToPlayerBase = 1; 
    public int currencyDroppedOnDeath = 10;

    [Header("Visuals & Audio")]
    public GameObject enemyModelPrefab; // If you have different models
    public AudioClip spawnSound;
    public AudioClip deathSound;
    public AudioClip hitSound;

    
    public bool isFlying = false;
    public float specialAbilityCooldown = 5f;
   
}
