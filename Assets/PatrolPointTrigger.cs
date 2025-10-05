using UnityEngine;

public class PatrolPointTrigger : MonoBehaviour
{
    public PatrolWithTriggers enemy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))  // or "Player" depending on your setup
        {
            enemy.OnReachPatrolPoint();
        }
    }
}