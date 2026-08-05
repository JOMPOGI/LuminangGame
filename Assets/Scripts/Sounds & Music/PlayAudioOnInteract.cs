using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(InteractableBase))]
public class PlayAudioOnInteract : MonoBehaviour
{
    private AudioSource audioSource;
    private InteractableBase interactable;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        interactable = GetComponent<InteractableBase>();
        
        if (interactable != null)
        {
            if (interactable.OnInteract == null) 
            {
                interactable.OnInteract = new UnityEvent();
            }
            interactable.OnInteract.AddListener(PlaySound);
        }
    }

    public void PlaySound()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }
}
