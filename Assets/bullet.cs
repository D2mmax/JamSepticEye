using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    public float lifetime = 5f;           // How long before the bullet auto-destroys
    public int damage = 1;                // Damage dealt to player

    [Header("Layers & Tags")]
    public LayerMask groundLayer;         // Set to your ground layer(s)
    public string playerTag = "Player";   // Tag used for the player

    private void Start()
    {
        // Destroy bullet after lifetime seconds to avoid memory leaks
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If hits ground, destroy bullet
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            Destroy(gameObject);
            return;
        }

        // If hits player, apply damage and destroy bullet
        if (collision.CompareTag(playerTag))
        {
            // Attempt to get a health/damageable component
            var health = collision.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
