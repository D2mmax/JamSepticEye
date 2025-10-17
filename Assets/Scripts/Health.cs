using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Events")]
    public UnityEvent onDeath;

    [Header("Eye Possession")]
    public EyePossession eyePossession; // assign in inspector if possible

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);

        // Notify the Eye if this object is being possessed
        if (eyePossession != null)
        {
            eyePossession.NotifyDamageTaken();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }

    public float NormalizedHealth => currentHealth / maxHealth;

    private void Die()
    {
        // Run any death events (for sounds, particles, etc.)
        onDeath?.Invoke();

        // Always safely detach the Eye before disabling the enemy
        if (eyePossession != null && eyePossession.IsPossessing() && eyePossession.IsPossessingEnemy(this.gameObject))
        {
            eyePossession.transform.SetParent(null);
            eyePossession.Release();
        }

        // Finally, deactivate the enemy after a short delay
        StartCoroutine(DeactivateAfterDelay(0.1f));
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
