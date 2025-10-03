using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Health targetHealth;
    public Image fillImage;

    void Update()
    {
        if (targetHealth != null)
            fillImage.fillAmount = targetHealth.NormalizedHealth;
    }

    public void SetTarget(Health newTarget)
    {
        targetHealth = newTarget;
    }
}
