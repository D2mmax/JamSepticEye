using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class PatrolWithTriggers : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Patrol Points")]
    public Transform[] patrolPoints;
    private int currentPointIndex = 0;
    private bool waiting = false;

    [Header("Detection")]
    public Collider2D detectionZone; // assign large trigger in inspector
    public string playerTag = "Player";
    public float minDistance = 1.5f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float fireRate = 1f;

    private SpriteRenderer sr;
    private Transform player;
    private bool chasingPlayer = false;
    private float fireCooldown = 0f;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;

        if (chasingPlayer && player != null)
        {
            HandleChasing();
            HandleShooting();
        }
        else if (!waiting)
        {
            Patrol();
        }
    }

    private void HandleChasing()
    {
        float dirX = player.position.x - transform.position.x;

        // Stop approaching if too close
        float moveDir = Mathf.Sign(dirX);
        if (Mathf.Abs(dirX) > minDistance)
            transform.position += new Vector3(moveDir * moveSpeed * Time.deltaTime, 0, 0);

        // Flip based on movement direction
        if (Mathf.Abs(dirX) > 0.01f)
            sr.flipX = moveDir > 0;
    }

    private void HandleShooting()
    {
        if (fireCooldown > 0f || player == null || bulletPrefab == null || firePoint == null) return;

        // Shoot towards player
        Vector2 dir = (player.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir * bulletSpeed;

        fireCooldown = fireRate;
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentPointIndex];
        float dirX = target.position.x - transform.position.x;
        float moveDir = Mathf.Sign(dirX);

        transform.position += new Vector3(moveDir * moveSpeed * Time.deltaTime, 0, 0);

        // Flip sprite based on movement direction
        if (Mathf.Abs(dirX) > 0.01f)
            sr.flipX = moveDir > 0;
    }

    public void OnReachPatrolPoint()
    {
        if (waiting) return;
        StartCoroutine(WaitThenNextPoint());
    }

    private IEnumerator WaitThenNextPoint()
    {
        waiting = true;
        yield return new WaitForSeconds(1f);
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        waiting = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            player = collision.transform;
            chasingPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            chasingPlayer = false;
            player = null;
        }
    }
}
