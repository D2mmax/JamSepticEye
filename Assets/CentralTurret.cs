using UnityEngine;
using UnityEngine.Events;

public class BossFightManager : MonoBehaviour
{
    [Header("Fight Settings")]
    public float baseFireRate = 2f;       // Starting fire interval
    public float fireRateDecrease = 0.05f; // How much the fire interval decreases per difficulty tick
    public float minFireRate = 0.5f;      // Fastest it can shoot
    public float difficultyIncreaseInterval = 10f; // How often difficulty scales (seconds)
    public float bulletForce = 5f;        // Speed of bullets
    public GameObject bulletPrefab;       // Bullet prefab
    public Transform turretTransform;     // Center turret position

    [Header("Enemy Spawning")]
    public Transform[] enemySpawners;     // Two spawners in the scene
    public GameObject enemyPrefab;        // Enemy to spawn
    public float enemySpawnInterval = 10f; // Spawn rate per spawner

    [Header("Boss Health")]
    public Health bossHealth;             // Central turret's health script
    public UnityEvent onBossDefeated;     // Inspector event when boss dies

    [Header("Events")]
    public UnityEvent onFightStarted;
    public UnityEvent onFightEnded;

    private bool fightActive = false;
    private float fightTimer = 0f;
    private float currentFireRate;
    private float nextFireTime = 0f;
    private float nextDifficultyTime = 0f;
    private float nextEnemySpawnTime = 0f;

    void Awake()
    {
        currentFireRate = baseFireRate;

        if (bossHealth != null)
            bossHealth.onDeath.AddListener(EndFight);
    }

    void Update()
    {
        if (!fightActive) return;

        fightTimer += Time.deltaTime;

        // Turret Shooting
        if (Time.time >= nextFireTime)
        {
            ShootInAllDirections();
            nextFireTime = Time.time + currentFireRate;
        }

        // Difficulty Scaling
        if (fightTimer >= nextDifficultyTime)
        {
            ScaleDifficulty();
            nextDifficultyTime += difficultyIncreaseInterval;
        }

        // Enemy Spawning
        if (Time.time >= nextEnemySpawnTime)
        {
            SpawnEnemies();
            nextEnemySpawnTime = Time.time + enemySpawnInterval;
        }
    }

    // Called by SurvivalTrigger or manually
    public void StartFight()
    {
        if (fightActive) return;

        fightActive = true;
        fightTimer = 0f;
        currentFireRate = baseFireRate;
        nextDifficultyTime = difficultyIncreaseInterval;
        nextEnemySpawnTime = Time.time + enemySpawnInterval;

        onFightStarted?.Invoke();
    }

    private void EndFight()
    {
        if (!fightActive) return;

        fightActive = false;
        onFightEnded?.Invoke();
        onBossDefeated?.Invoke();
    }

    private void ShootInAllDirections()
    {
        int bulletCount = 12; // 360°/12 = 30° spread
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * (360f / bulletCount);
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector3 dir = rotation * Vector3.right;

            GameObject bullet = Instantiate(bulletPrefab, turretTransform.position, rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.AddForce(dir * bulletForce, ForceMode2D.Impulse);
        }
    }

    private void ScaleDifficulty()
    {
        currentFireRate = Mathf.Max(minFireRate, currentFireRate - fireRateDecrease);
    }

    private void SpawnEnemies()
    {
        foreach (Transform spawner in enemySpawners)
        {
            Instantiate(enemyPrefab, spawner.position, Quaternion.identity);
        }
    }
}
