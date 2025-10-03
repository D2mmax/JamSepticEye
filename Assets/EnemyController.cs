using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public Transform groundCheckPoint; // empty child object under feet
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer; // assign your ground layer

    private Rigidbody2D rb;
    private PossessableEnemy possessable;
    private bool isGrounded;

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
    }

    private void HandleMovement()
    {
        float move = Input.GetAxisRaw("Horizontal");
        Vector2 vel = rb.linearVelocity;
        vel.x = move * moveSpeed;
        rb.linearVelocity = vel;
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void CheckGrounded()
    {
        if (groundCheckPoint == null) return;

        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    private void HandleInteract()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Define where the check happens: slightly in front of the enemy
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
}
