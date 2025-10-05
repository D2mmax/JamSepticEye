using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public LayerMask horizontalCollisionLayer;
    public float wallCheckDistance = 0.1f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public int maxJumpCount = 2;
    public float jumpHoldForce = 3f;
    public float maxJumpHoldTime = 0.2f;

    private Rigidbody2D rb;
    private PossessableEnemy possessable;
    private bool isGrounded;
    private bool isTouchingWall;

    private int jumpCount = 0;
    private bool isJumping = false;
    private float jumpHoldTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        possessable = GetComponent<PossessableEnemy>();
    }

    void Update()
    {
        if (!possessable.IsPossessed) return;

        HandleMovement();
        HandleJump();
        HandleInteract();
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

    private void HandleJump()
    {
        // Start jump
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++;
            isJumping = true;
            jumpHoldTimer = 0f;
        }

        // Variable jump while holding
        if (isJumping && Input.GetButton("Jump") && jumpHoldTimer < maxJumpHoldTime)
        {
            rb.AddForce(Vector2.up * jumpHoldForce * Time.fixedDeltaTime, ForceMode2D.Impulse);
            jumpHoldTimer += Time.fixedDeltaTime;
        }

        if (Input.GetButtonUp("Jump"))
            isJumping = false;

        // Reset jump count when grounded
        if (isGrounded && rb.linearVelocity.y <= 0f)
        {
            jumpCount = 0;
            isJumping = false;
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
        if (dir > 0f)
            isTouchingWall = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, horizontalCollisionLayer);
        else if (dir < 0f)
            isTouchingWall = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, horizontalCollisionLayer);
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
