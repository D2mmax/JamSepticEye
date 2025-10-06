using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class SurvivalTrigger : MonoBehaviour
{
    [Header("References")]
    public BossFightManager bossFightManager; // Central turret or boss
    public Camera bossCamera; // Assign in inspector or auto-find

    [Header("Events")]
    public UnityEvent onPlayerEntered; // Custom inspector events

    private bool fightStarted = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-acquire scene references in case of scene reload
        if (bossFightManager == null)
            bossFightManager = FindObjectOfType<BossFightManager>();

        if (bossCamera == null)
            bossCamera = GameObject.Find("BossCamera")?.GetComponent<Camera>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (fightStarted) return;
        if (!collision.CompareTag("Player")) return;

        fightStarted = true;

        // Start boss fight
        if (bossFightManager != null)
            bossFightManager.StartFight();

        // Switch to boss camera
        if (bossCamera != null)
            CameraManager.Instance.SwitchCamera(bossCamera);

        // Fire any inspector events
        onPlayerEntered?.Invoke();
    }
}
