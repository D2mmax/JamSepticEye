using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button playButton;
    public Button optionsButton;
    public Button quitButton;

    [Header("Scene Settings")]
    public string sceneToLoad = "SampleScene"; // Scene hitting play will go to

    void Start()
    {
        if (playButton != null) playButton.onClick.AddListener(PlayGame);
        if (optionsButton != null) optionsButton.onClick.AddListener(OpenOptions);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
    }

    void PlayGame()
    {
        Debug.Log("Play button clicked — loading scene: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }

    void OpenOptions()
    {
        Debug.Log("Options button clicked — show options menu");
        // TODO: Replace this with your options menu logic
        // For now you could enable a UI panel, like:
        // optionsMenuPanel.SetActive(true);
    }

    void QuitGame()
    {
        Debug.Log("Quit button clicked — exiting game");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in editor
#else
        Application.Quit(); // Quits the build
#endif
    }
}
