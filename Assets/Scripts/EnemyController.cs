using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public Transform groundCheckPoint; // empty child object under feet
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer; // assign your ground layer
    public LayerMask horizontalCollisionLayer; // assign Ground + Interactable here
    public float wallCheckDistance = 0.1f; // distance for wall raycast

    private Rigidbody2D rb;
    private PossessableEnemy possessable;
    private bool isGrounded;
    private bool isTouchingWall;

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

    private void FixedUpdate()
    {
        CheckGrounded();
        CheckWall();
    }

    private void HandleMovement()
    {
        float move = Input.GetAxisRaw("Horizontal");

        // Prevent moving into a wall
        if ((move > 0 && isTouchingWall) || (move < 0 && isTouchingWall))
            move = 0f;

        Vector2 vel = rb.linearVelocity;
        vel.x = move * moveSpeed;
        rb.linearVelocity = vel;
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && !isTouchingWall)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void CheckGrounded()
    {
        if (groundCheckPoint == null) return;

        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    private void CheckWall()
{
    float direction = Input.GetAxisRaw("Horizontal");

    // Only cast a ray if actually moving
    if (direction > 0f)
        isTouchingWall = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, horizontalCollisionLayer);
    else if (direction < 0f)
        isTouchingWall = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, horizontalCollisionLayer);
    else
        isTouchingWall = false;
}

    private void HandleInteract()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector2 checkPosition = (Vector2)transform.position + new Vector2(Mathf.Sign(transform.localScale.x) * 1f, 0f);
            float checkRadius = 0.5f;

            Collider2D hit = Physics2D.OverlapCircle(checkPosition, checkRadius, LayerMask.GetMask("Interactable"));

            if (hit != null)
            {
                Door door = hit.GetComponent<Door>();
                if (door != null)
                    door.Open();
            }
        }
    }

    // Optional: visualize wall raycast in Scene view
    void OnDrawGizmos()
    {
        if (possessable != null && possessable.IsPossessed)
        {
            Gizmos.color = Color.red;
            float direction = Input.GetAxisRaw("Horizontal");
            if (direction > 0)
                Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheckDistance);
            else if (direction < 0)
                Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallCheckDistance);
        }
    }
}
