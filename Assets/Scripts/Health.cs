using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

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
        if (currentHealth <= 0)
        {
            // Detach the eye if this enemy is being possessed
            DetachEye();

            onDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }

    public float NormalizedHealth => currentHealth / maxHealth;

    public void DetachEye()
    {
        if (eyePossession != null && eyePossession.IsPossessing())
        {
            // Detach the eye so it won't be affected by enemy's inactive state
            eyePossession.transform.SetParent(null);

            // Stop possessing this enemy
            eyePossession.Release();
        }
    }
}
