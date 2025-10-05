using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button playButton;
    public Button quitButton;

    [Header("Button Settings")]
    public string sceneToLoad = "SampleScene"; // Scene hitting play will go to

    void Start()
    {
        if (playButton != null) playButton.onClick.AddListener(PlayGame);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
    }

    void PlayGame()
    {
        Debug.Log("Play button clicked — loading scene: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad); // Loads the game
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
