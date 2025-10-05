using UnityEngine;
using UnityEngine.Audio;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer Mixer;
    [SerializeField] private AudioMixMode MixMode;

    private void Start()
    {
        // Initialize the volume to the last saved value or the default (1) if none exists
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1);
        Mixer.SetFloat("Volume", Mathf.Log10(savedVolume) * 20);
    }

    public void OnChangeSlider(float Value)
    {
        // Save and apply the volume change based on the selected mode
        switch (MixMode)
        {
            case AudioMixMode.LinearMixerVolume:
                Mixer.SetFloat("Volume", (-80 + Value * 80)); // Linear scale from -80dB to 0dB
                break;
            case AudioMixMode.LogarithmicMixerVolume:
                Mixer.SetFloat("Volume", Mathf.Log10(Value) * 20); // Logarithmic scale
                break;
        }

        // Save the volume for future sessions
        PlayerPrefs.SetFloat("Volume", Value);
        PlayerPrefs.Save();
    }

    public enum AudioMixMode
    {
        LinearMixerVolume,
        LogarithmicMixerVolume
    }
}