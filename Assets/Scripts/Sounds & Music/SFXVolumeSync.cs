using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXVolumeSync : MonoBehaviour
{
    private AudioSource audioSource;
    private float baseVolume = 1.0f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        baseVolume = audioSource.volume; // Capture the designed volume (e.g., 0.5f)
    }

    private void Start()
    {
        UpdateVolume();
        AudioManager.onSFXVolumeChange += UpdateVolume;
    }

    private void UpdateVolume()
    {
        if (audioSource != null && AudioManager.instance != null)
        {
            // Final volume = (Designed Volume) * (Slider Percentage)
            audioSource.volume = baseVolume * AudioManager.instance.sfxVolume;
        }
    }

    private void OnDestroy()
    {
        AudioManager.onSFXVolumeChange -= UpdateVolume;
    }
}
