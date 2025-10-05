using UnityEngine;

public class Spikes : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damageAmount = 20f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Try to get a Health component on the object that entered
        Health targetHealth = collision.GetComponent<Health>();
        
        // If not found on the root, try parent (useful for enemies or players with multiple colliders)
        if (targetHealth == null)
            targetHealth = collision.GetComponentInParent<Health>();

        // If a Health component exists, apply damage
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damageAmount);
        }
    }
}
