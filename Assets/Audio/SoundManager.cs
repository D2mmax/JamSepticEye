using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    ENTERPOSSESSION,
    EXITPOSSESSION,
    GUNSHOT,
    EXPLOSION
}

[System.Serializable]
public class SoundEntry
{
    public SoundType soundType;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private List<SoundEntry> sounds = new List<SoundEntry>();
    private static SoundManager instance;
    private AudioSource audioSource;
    private Dictionary<SoundType, SoundEntry> soundDict;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Build lookup table for quick access
        soundDict = new Dictionary<SoundType, SoundEntry>();
        foreach (var s in sounds)
        {
            if (!soundDict.ContainsKey(s.soundType))
                soundDict.Add(s.soundType, s);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType type, float volumeMultiplier = 1f)
    {
        if (instance == null || !instance.soundDict.ContainsKey(type)) return;

        var sound = instance.soundDict[type];
        instance.audioSource.PlayOneShot(sound.clip, sound.volume * volumeMultiplier);
    }
}
