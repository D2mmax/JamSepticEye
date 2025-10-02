using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EyeController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float acceleration = 10f;   // how quickly velocity ramps
    public float drag = 5f;            // how floaty / slippery movement feels

    private Rigidbody2D rb;
    private Vector2 input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // no gravity, always floating
    }

    void Update()
    {
        // Gather input
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        input = new Vector2(x, y).normalized;
    }

  void FixedUpdate()
{
    Vector2 targetVel = input * moveSpeed;

    // If moving, use acceleration; if not, use drag for slowing down
    float lerpFactor = (input.magnitude > 0.1f) ? acceleration : drag;

    // Ghosty easing: exponential-style interpolation
    rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVel, 1f - Mathf.Exp(-lerpFactor * Time.fixedDeltaTime));
}

}
