using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;          // Current thing camera should follow
    public float smoothTime = 0.2f;   // How smooth the follow is
    public Vector3 offset = new Vector3(0, 1, -10); // Camera offset

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        // Desired position
        Vector3 targetPos = target.position + offset;

        // Smooth damp toward target
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }

    // Called by possession system
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
