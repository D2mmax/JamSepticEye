using UnityEngine;
using System.Collections;

public class EyePossession : MonoBehaviour
{
    private EyeController eyeController;
    private bool inRange = false;
    private PossessableEnemy targetEnemy;
    private Coroutine possessionRoutine;
    private CapsuleCollider2D eyeCollider;
    [Header("Hover settings")]
    public float hoverAmplitude = 0.2f;
    public float hoverFrequency = 2f;
    public float swayMultiplier = 0.3f;
    public float swaySmoothTime = 0.1f;

    private Vector3 hoverOffset;
    private Vector3 swayVelocity; 

    public float possessTime = 1f;
    private bool isPossessing = false;
    public Health eyeHealth;
    public HealthBar EyeHealthBar;
    public HealthBar hostHealthBar;

    public float possessionDrainRate = 10f; // health drained per second

    void Awake()
    {
        eyeController = GetComponent<EyeController>();
        eyeCollider = GetComponent<CapsuleCollider2D>(); // grab the collider
    }

    void Update()
    {
        if (inRange && targetEnemy != null && !isPossessing)
        {
            if (Input.GetKeyDown(KeyCode.E))
                possessionRoutine = StartCoroutine(PossessEnemyRoutine(targetEnemy));
            if (Input.GetKeyUp(KeyCode.E) && possessionRoutine != null)
            {
                StopCoroutine(possessionRoutine);
                possessionRoutine = null;
            }
        }

        if (isPossessing && targetEnemy != null)
        {
            Health hostHealth = targetEnemy.GetComponent<Health>();
            if (hostHealth != null)
            {
                hostHealth.TakeDamage(possessionDrainRate * Time.deltaTime);

                if (hostHealth.currentHealth <= 0)
                    Release();
            }
        }
    }

    private IEnumerator PossessEnemyRoutine(PossessableEnemy enemy)
    {
        float timer = 0f;
        while (timer < possessTime)
        {
            if (!Input.GetKey(KeyCode.E)) yield break;
            timer += Time.deltaTime;
            yield return null;
        }

        Possess(enemy);
    }

    private void Possess(PossessableEnemy enemy)
{
    isPossessing = true;
    eyeController.enabled = false;

    // Disable collisions if needed
    if (eyeCollider != null)
        eyeCollider.enabled = false;

    // Set rendering order above the host
    SpriteRenderer sr = GetComponent<SpriteRenderer>();
    if (sr != null)
        sr.sortingOrder = 1; // on top of enemy

    // Change enemy tag to player
    enemy.gameObject.tag = "Player";

    targetEnemy = enemy;
    transform.position = enemy.shoulderAnchor.position;

    enemy.OnPossessed();

    // Camera follow & health bar setup
    hostHealthBar.gameObject.SetActive(true);
    hostHealthBar.SetTarget(enemy.GetComponent<Health>());
    Camera.main.GetComponent<CameraFollow>().SetTarget(enemy.transform);
}

    private void OnTriggerEnter2D(Collider2D collision)
{
    // Ignore all new enemies if already possessing
    if (isPossessing) return;

    PossessableEnemy enemy = collision.GetComponentInParent<PossessableEnemy>();
    if (enemy != null)
    {
        inRange = true;
        targetEnemy = enemy;
    }
}

private void OnTriggerExit2D(Collider2D collision)
{
    // Ignore if already possessing
    if (isPossessing) return;

    PossessableEnemy enemy = collision.GetComponentInParent<PossessableEnemy>();
    if (enemy == targetEnemy)
    {
        inRange = false;
        targetEnemy = null;
    }
}

    public void Release()
{
    if (!isPossessing) return;

    isPossessing = false;
    eyeController.enabled = true;

    if (eyeCollider != null)
        eyeCollider.enabled = true;

    // Reset rendering order
    SpriteRenderer sr = GetComponent<SpriteRenderer>();
    if (sr != null)
        sr.sortingOrder = 0; // back to default

    // Reset enemy tag
    if (targetEnemy != null)
        targetEnemy.gameObject.tag = "Enemy";

    transform.SetParent(null);

    if (targetEnemy != null)
        targetEnemy.OnReleased();

    hostHealthBar.gameObject.SetActive(false);

    Camera.main.GetComponent<CameraFollow>().SetTarget(this.transform);
}
    void LateUpdate()
    {
        if (isPossessing && targetEnemy != null)
        {
            Rigidbody2D hostRb = targetEnemy.GetComponent<Rigidbody2D>();

            // Base position: shoulder
            Vector3 basePos = targetEnemy.shoulderAnchor.position;

            // Vertical sinusoidal hover
            float hoverY = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;

            // Horizontal + vertical sway based on host linearVelocity
            Vector2 hostVel = hostRb.linearVelocity;
            Vector3 targetSway = new Vector3(
                hostVel.x * swayMultiplier,   // sway right/left
                hostVel.y * swayMultiplier,   // sway up/down
                0
            );

            // Smoothly interpolate sway
            Vector3 smoothSway = Vector3.SmoothDamp(hoverOffset, targetSway, ref swayVelocity, swaySmoothTime);

            // Combine hover + sway
            hoverOffset = new Vector3(smoothSway.x, smoothSway.y + hoverY, 0);

            // Apply final position
            transform.position = basePos + hoverOffset;
        }
    }
}
