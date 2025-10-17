using UnityEngine;

public class Spikes : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damageAmount = 20f;
    public string[] validTags = { "Player", "Enemy" }; // Only these can take damage

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only damage if the object has a valid tag
        foreach (var tag in validTags)
        {
            if (collision.CompareTag(tag))
            {
                Health targetHealth = collision.GetComponent<Health>() ?? collision.GetComponentInParent<Health>();
                if (targetHealth != null)
                    targetHealth.TakeDamage(damageAmount);
                break;
            }
        }
    }
}
