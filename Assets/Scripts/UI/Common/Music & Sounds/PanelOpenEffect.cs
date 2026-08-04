using UnityEngine;
using System.Collections;

// Attach this script to any Panel you want to animate/play a sound when it opens.
public class PanelOpenEffect : MonoBehaviour
{
    [Header("Sound Effect")]
    public AudioSource sfxSource;
    public AudioClip openSFX;

    [Header("Animation Settings")]
    public bool animateOnEnable = true;
    public float startDelay = 0f; // Delay before pop up happens
    public float animationDuration = 0.3f;
    public float overshootScale = 1.1f;

    void OnEnable()
    {
        if (animateOnEnable)
        {
            // Immediately hide it so it doesn't flash before the delay finishes
            transform.localScale = Vector3.zero;
            StartCoroutine(PlayEffectDelayed());
        }
    }

    public void PlayEffect()
    {
        StartCoroutine(PlayEffectDelayed());
    }

    private IEnumerator PlayEffectDelayed()
    {
        if (startDelay > 0f)
        {
            yield return new WaitForSeconds(startDelay);
        }

        // Play the sound after the delay
        if (sfxSource != null && openSFX != null)
        {
            sfxSource.PlayOneShot(openSFX);
        }

        // Play the bounce animation
        StartCoroutine(AnimatePanelIn());
    }

    private IEnumerator AnimatePanelIn()
    {
        transform.localScale = Vector3.zero;
        
        float elapsed = 0f;

        // Pop up and overshoot slightly
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            
            // Bouncy math (Ease out back)
            float scale = 1 + (overshootScale - 1f) * Mathf.Sin(t * Mathf.PI); 
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * overshootScale, t);
            yield return null;
        }

        // Settle back to exactly 1
        elapsed = 0f;
        while (elapsed < 0.1f)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.one * overshootScale, Vector3.one, elapsed / 0.1f);
            yield return null;
        }

        transform.localScale = Vector3.one;
    }
}
