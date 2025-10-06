using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;
    public Transform spawnPoint;  // if null, will use spawner’s transform

    public void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        Transform point = spawnPoint != null ? spawnPoint : transform;
        Instantiate(enemyPrefab, point.position, point.rotation);
    }
}