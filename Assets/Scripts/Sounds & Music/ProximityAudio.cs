using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SFXVolumeSync))]
public class ProximityAudio : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("If true, the audio will loop continuously instead of playing at intervals.")]
    public bool isContinuous = false;
    
    [Tooltip("If true, this sound will only play ONCE and never again.")]
    public bool playOnce = false;
    
    [Tooltip("How often the sound plays (in seconds) if not continuous.")]
    public float playInterval = 3f;

    [Tooltip("The maximum distance from the player where the sound can be heard.")]
    public float triggerDistance = 20f;

    [Tooltip("Optional: Assign the player transform. If left empty, it will auto-find by 'Player' tag.")]
    public Transform player;

    private AudioSource audioSource;
    private float timer;
    private Transform playerTransform;
    private bool wasInRange = false;
    private static float globalNextAllowedTime = 0f;

    [Tooltip("If true, this sound will share a global cooldown with other sounds so they alternate (salit-salitan).")]
    public bool useGlobalCooldown = true;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Automatically configure the AudioSource for 3D spatial sound
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 2f;
        audioSource.maxDistance = triggerDistance;
        
        // Linear rolloff provides a strict boundary. When distance == maxDistance, volume is 0.
        audioSource.rolloffMode = AudioRolloffMode.Linear; 
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        if (player != null)
        {
            playerTransform = player;
        }
        else
        {
            GameObject pGameObject = GameObject.FindGameObjectWithTag("Player");
            if (pGameObject != null)
            {
                playerTransform = pGameObject.transform;
            }
        }

        if (isContinuous)
        {
            audioSource.loop = true;
            // Continuous sounds only play when in range now
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool inRange = distance <= triggerDistance;

        if (inRange)
        {
            if (!wasInRange)
            {
                wasInRange = true;
                timer = Random.Range(1f, 3f); // Small delay when first entering range
                if (isContinuous)
                {
                    audioSource.Play(); // Play immediately when entering range
                }
            }

            if (!isContinuous)
            {
                timer -= Time.deltaTime;
                
                // Only play if timer is done AND the global cooldown allows it
                if (timer <= 0f && (!useGlobalCooldown || Time.time >= globalNextAllowedTime))
                {
                    audioSource.Play();
                    
                    if (playOnce)
                    {
                        // Stop this component entirely if it's meant to play only once
                        this.enabled = false;
                        return;
                    }

                    // Set global cooldown so no other animal plays for at least 5 seconds
                    if (useGlobalCooldown)
                    {
                        globalNextAllowedTime = Time.time + 5f; 
                    }
                    
                    // Reset our own local timer so we wait before checking again
                    timer = playInterval + Random.Range(0f, 2f);
                }
            }
        }
        else
        {
            if (wasInRange)
            {
                wasInRange = false;
                if (isContinuous)
                {
                    audioSource.Stop(); // Strictly stop the continuous sound when walking away
                }
                else if (audioSource.isPlaying)
                {
                    audioSource.Stop(); // Strictly stop any interval sound immediately
                }
            }
        }
    }
}
