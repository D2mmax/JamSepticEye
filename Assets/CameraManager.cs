using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
    }

    public void SwitchCamera(Camera newCam)
    {
        if (newCam == null) return;

        foreach (Camera cam in Camera.allCameras)
            cam.enabled = false;

        newCam.enabled = true;
    }

    public Camera GetMainCamera()
    {
        return Camera.main;
    }
}
