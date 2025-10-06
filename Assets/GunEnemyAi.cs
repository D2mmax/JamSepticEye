using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class PatrolWithTriggers : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Patrol Points")]
    public Transform[] patrolPoints;
    public int currentPointIndex = 0;
    private bool waiting = false;

    [Header("Detection")]
    public Collider2D detectionZone;
    public string playerTag = "Player";
    public float minDistance = 1.5f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float fireRate = 1f;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Transform player;
    private bool chasingPlayer = false;
    private float fireCooldown = 0f;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
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

        // Only move if too far from player
        float moveDir = 0f;
        if (Mathf.Abs(dirX) > minDistance)
            moveDir = Mathf.Sign(dirX);

        // Apply velocity instead of transform
        Vector2 vel = rb.linearVelocity;
        vel.x = moveDir * moveSpeed;
        rb.linearVelocity = vel;

        // Flip sprite based on direction
        if (Mathf.Abs(moveDir) > 0.01f)
            sr.flipX = moveDir > 0;
    }

    private void HandleShooting()
    {
        if (fireCooldown > 0f || player == null || bulletPrefab == null || firePoint == null) return;

        Vector2 dir = (player.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
            bulletRb.linearVelocity = dir * bulletSpeed;

        fireCooldown = fireRate;
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentPointIndex];
        float dirX = target.position.x - transform.position.x;
        float moveDir = 0f;

        if (Mathf.Abs(dirX) > 0.01f)
            moveDir = Mathf.Sign(dirX);

        // Use Rigidbody velocity
        Vector2 vel = rb.linearVelocity;
        vel.x = moveDir * moveSpeed;
        rb.linearVelocity = vel;

        // Flip sprite
        if (Mathf.Abs(moveDir) > 0.01f)
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
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stop moving while waiting
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
