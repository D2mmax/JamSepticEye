using System.Collections;
using UnityEngine;

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

    [Header("Passive Heal")]
    public float healAmountPerSecond = 10f;
    public float healDelay = 5f; // seconds after last damage
    private float lastDamageTime = 0f;

    void Awake()
    {
        eyeController = GetComponent<EyeController>();
        eyeCollider = GetComponent<CapsuleCollider2D>();
    }
    public bool IsPossessing()
{
    return isPossessing;
}

    void Update()
    {
        // Track passive healing
        HandlePassiveHealing();

        if (inRange && targetEnemy != null && !isPossessing)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
                possessionRoutine = StartCoroutine(PossessEnemyRoutine(targetEnemy));
            
            if (Input.GetKeyUp(KeyCode.LeftShift) && possessionRoutine != null)
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

    private void HandlePassiveHealing()
{
    if (eyeHealth == null) return;

    if (Time.time - lastDamageTime >= healDelay && eyeHealth.currentHealth < eyeHealth.maxHealth)
    {
        eyeHealth.Heal(healAmountPerSecond * Time.deltaTime);
        // No need to update the HealthBar manually; it updates itself in Update()
    }
}

    // Call this function whenever the eye takes damage
    public void NotifyDamageTaken()
    {
        lastDamageTime = Time.time;
    }

    private IEnumerator PossessEnemyRoutine(PossessableEnemy enemy)
    {
        float timer = 0f;
        while (timer < possessTime)
        {
            if (!Input.GetKey(KeyCode.LeftShift)) yield break;
            timer += Time.deltaTime;
            yield return null;
        }

        Possess(enemy);
    }
    public bool IsPossessingEnemy(GameObject enemy)
{
    return isPossessing && targetEnemy != null && targetEnemy.gameObject == enemy;
}

    private void Possess(PossessableEnemy enemy)
    {
        isPossessing = true;
        eyeController.enabled = false;

        if (eyeCollider != null)
            eyeCollider.enabled = false;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sortingOrder = 1;

        enemy.gameObject.tag = "Player";
        enemy.gameObject.layer = LayerMask.NameToLayer("Eye");

        PatrolWithTriggers patrolScript = enemy.GetComponent<PatrolWithTriggers>();
        if (patrolScript != null)
            patrolScript.enabled = false;

        targetEnemy = enemy;

        transform.SetParent(enemy.shoulderAnchor);
        transform.localPosition = Vector3.zero;

        enemy.OnPossessed();

        hostHealthBar.gameObject.SetActive(true);
        hostHealthBar.SetTarget(enemy.GetComponent<Health>());
        Camera.main.GetComponent<CameraFollow>().SetTarget(enemy.transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sortingOrder = 0;

        if (targetEnemy != null)
            targetEnemy.gameObject.tag = "Enemy";

        if (targetEnemy != null)
            targetEnemy.gameObject.layer = LayerMask.NameToLayer("Enemy");

        PatrolWithTriggers patrolScript = targetEnemy.GetComponent<PatrolWithTriggers>();
        if (patrolScript != null)
            patrolScript.enabled = true;

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
            Vector3 basePos = targetEnemy.shoulderAnchor.position;
            float hoverY = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;

            Vector2 hostVel = hostRb.linearVelocity;
            Vector3 targetSway = new Vector3(
                hostVel.x * swayMultiplier,
                hostVel.y * swayMultiplier,
                0
            );

            Vector3 smoothSway = Vector3.SmoothDamp(hoverOffset, targetSway, ref swayVelocity, swaySmoothTime);
            hoverOffset = new Vector3(smoothSway.x, smoothSway.y + hoverY, 0);

            transform.position = basePos + hoverOffset;
        }
    }
}
