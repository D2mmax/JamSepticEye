using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    private Rigidbody2D rb;
    private PossessableEnemy possessable;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        possessable = GetComponent<PossessableEnemy>();
    }

    void Update()
    {
        if (!possessable.IsPossessed) return;

        float move = Input.GetAxisRaw("Horizontal");
        Vector2 vel = rb.linearVelocity;
        vel.x = move * moveSpeed;
        rb.linearVelocity = vel;

        if (Input.GetButtonDown("Jump"))
        {
            // Basic jump (no ground check yet)
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
