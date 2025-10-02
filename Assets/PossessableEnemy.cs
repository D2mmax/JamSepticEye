using UnityEngine;

public class PossessableEnemy : MonoBehaviour
{
    public Transform shoulderAnchor;
    private bool isPossessed = false;

    public void OnPossessed()
    {
        isPossessed = true;
    }

    public void OnReleased()
    {
        isPossessed = false;
    }

    public bool IsPossessed => isPossessed;
}
