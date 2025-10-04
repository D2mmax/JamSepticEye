using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EyeController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float drag = 5f;

    private Rigidbody2D rb;
    private Vector2 input;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // floaty
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Gather input
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        // Update animator speed
        if (animator != null)
            animator.SetFloat("Speed", rb.linearVelocity.magnitude);
            animator.SetBool("isMoving", input.magnitude > 0.1f);

        // Flip sprite left/right if moving horizontally
        if (spriteRenderer != null)
        {
            if (rb.linearVelocity.x > 0.1f)
                spriteRenderer.flipX = false; // facing right
            else if (rb.linearVelocity.x < -0.1f)
                spriteRenderer.flipX = true; // facing left
        }
    }

    void FixedUpdate()
    {
        Vector2 targetVel = input * moveSpeed;
        float lerpFactor = (input.magnitude > 0.1f) ? acceleration : drag;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVel, 1f - Mathf.Exp(-lerpFactor * Time.fixedDeltaTime));
    }
}
