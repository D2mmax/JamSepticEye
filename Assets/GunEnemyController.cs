using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GunEnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public float wallCheckDistance = 0.1f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public int maxJumpCount = 2;
    public float jumpHoldForce = 3f;
    public float maxJumpHoldTime = 0.2f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    [Header("Animation")]
    public Animator animator; // Assign in inspector
    private const string IsMovingParam = "isMoving";

    [Header("AI Reference")]
    public PatrolWithTriggers aiScript; // Assign AI script in inspector

    private Rigidbody2D rb;
    private PossessableEnemy possessable;
    private SpriteRenderer spriteRenderer;

    private int jumpCount = 0;
    private bool isJumping = false;
    private float jumpHoldTimer = 0f;
    private bool isGrounded;
    private bool isTouchingWall;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        possessable = GetComponent<PossessableEnemy>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
{
    if (possessable.IsPossessed)
    {
        HandleMovement();
        HandleJump();
        HandleShooting();
        HandleInteract();
    }

    UpdateAnimation();   // <-- Always update animation
    HandleFlipping();    // <-- Always update flipping
}

    void FixedUpdate()
    {
        CheckGrounded();
        CheckWall();
    }

    private void HandleMovement()
    {
        float move = Input.GetAxisRaw("Horizontal");
        if ((move > 0 && isTouchingWall) || (move < 0 && isTouchingWall))
            move = 0f;

        Vector2 vel = rb.linearVelocity;
        vel.x = move * moveSpeed;
        rb.linearVelocity = vel;
    }

   private void UpdateAnimation()
{
    if (spriteRenderer == null || animator == null) return;

    // Consider moving if velocity.x is above threshold
    bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
    animator.SetBool("isMoving", isMoving);
}

    private void HandleFlipping()
    {
        if (spriteRenderer == null) return;

        float horizontalSpeed = 0f;

        if (possessable.IsPossessed)
            horizontalSpeed = rb.linearVelocity.x;
        else if (aiScript != null)
            horizontalSpeed = (aiScript.patrolPoints.Length > 0)
                ? Mathf.Sign(aiScript.patrolPoints[aiScript.currentPointIndex].position.x - transform.position.x) * aiScript.moveSpeed
                : 0f;

        if (Mathf.Abs(horizontalSpeed) > 0.1f)
            spriteRenderer.flipX = horizontalSpeed > 0f;
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++;
            isJumping = true;
            jumpHoldTimer = 0f;
        }

        if (isJumping && Input.GetButton("Jump") && jumpHoldTimer < maxJumpHoldTime)
        {
            rb.AddForce(Vector2.up * jumpHoldForce * Time.fixedDeltaTime, ForceMode2D.Impulse);
            jumpHoldTimer += Time.fixedDeltaTime;
        }

        if (Input.GetButtonUp("Jump"))
            isJumping = false;

        if (isGrounded && rb.linearVelocity.y <= 0f)
        {
            jumpCount = 0;
            isJumping = false;
        }
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0) && bulletPrefab != null && firePoint != null)
        {
            Vector3 mouseWorld3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mouseWorld = new Vector2(mouseWorld3D.x, mouseWorld3D.y);
            Vector2 direction = (mouseWorld - (Vector2)firePoint.position).normalized;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.linearVelocity = direction * bulletSpeed;
            SoundManager.PlaySound(SoundType.GUNSHOT);
        }
    }

    private void CheckGrounded()
    {
        if (groundCheckPoint == null) return;
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    private void CheckWall()
    {
        float dir = Input.GetAxisRaw("Horizontal");
        if (dir > 0)
            isTouchingWall = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer);
        else if (dir < 0)
            isTouchingWall = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer);
        else
            isTouchingWall = false;
    }

    private void HandleInteract()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector2 checkPos = (Vector2)transform.position + new Vector2(Mathf.Sign(transform.localScale.x) * 1f, 0f);
            float radius = 0.5f;
            Collider2D hit = Physics2D.OverlapCircle(checkPos, radius, LayerMask.GetMask("Interactable"));
            if (hit != null)
            {
                Door door = hit.GetComponent<Door>();
                if (door != null) door.Open();
            }
        }
    }
}
