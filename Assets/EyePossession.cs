using UnityEngine;
using System.Collections;

public class EyePossession : MonoBehaviour

{
    
    private EyeController eyeController;
    private bool inRange = false;
    private PossessableEnemy targetEnemy;
    private Coroutine possessionRoutine;
    private CircleCollider2D eyeCollider;
    [Header("Hover settings")]
    public float hoverAmplitude = 0.2f;      // vertical bob height
    public float hoverFrequency = 2f;        // vertical bob speed
    public float swayMultiplier = 0.3f;      // how much it sways based on host movement
    public float swaySmoothTime = 0.1f;      // smoothing for sway motion

    private Vector3 hoverOffset;
    private Vector3 swayVelocity; 


    public float possessTime = 1f;
    private bool isPossessing = false;
    

    void Awake()
    {
        eyeController = GetComponent<EyeController>();
       
        eyeCollider = GetComponent<CircleCollider2D>(); // grab the collider
    }

    void Update()
    {
        if (inRange && targetEnemy != null && !isPossessing)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                possessionRoutine = StartCoroutine(PossessEnemyRoutine(targetEnemy));
            }
            if (Input.GetKeyUp(KeyCode.E) && possessionRoutine != null)
            {
                StopCoroutine(possessionRoutine);
                possessionRoutine = null;
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

        // Success!
        Possess(enemy);
    }

    private void Possess(PossessableEnemy enemy)
{
    isPossessing = true;
    eyeController.enabled = false; // stop free movement
    if (eyeCollider != null) eyeCollider.enabled = false;

    targetEnemy = enemy;

    // Snap immediately
    transform.position = enemy.shoulderAnchor.position;
    transform.SetParent(enemy.shoulderAnchor); // optional

    enemy.OnPossessed();
}


    private void OnTriggerEnter2D(Collider2D collision)
    {
        PossessableEnemy enemy = collision.GetComponentInParent<PossessableEnemy>();
        if (enemy != null)
        {
            inRange = true;
            targetEnemy = enemy;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
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
    if (eyeCollider != null) eyeCollider.enabled = true;
    transform.SetParent(null);
    if (targetEnemy != null)
        targetEnemy.OnReleased();
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
