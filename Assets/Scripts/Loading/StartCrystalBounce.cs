using UnityEngine;
using System.Collections;

public class StartCrystalBounce : MonoBehaviour
{
    public Animator crystal1;
    public Animator crystal2;
    public Animator crystal3;
    
    [Header("Bounce Settings")]
    [Tooltip("Delay between each crystal starting to bounce")]
    public float delayBetweenBounces = 0.15f;
    public float bounceHeight = 19f;
    public float bounceDuration = 0.5f;
    public float pauseDuration = 0.25f;

    public void StartBounce()
    {
        StopAllCoroutines();
        StartCoroutine(SequenceRoutine());
    }

    private IEnumerator SequenceRoutine()
    {
        while (true)
        {
            // Start all three in parallel with a slight stagger
            if (crystal1 != null) StartCoroutine(SingleBounce(crystal1.GetComponent<RectTransform>()));
            yield return new WaitForSeconds(delayBetweenBounces);
            
            if (crystal2 != null) StartCoroutine(SingleBounce(crystal2.GetComponent<RectTransform>()));
            yield return new WaitForSeconds(delayBetweenBounces);
            
            if (crystal3 != null) StartCoroutine(SingleBounce(crystal3.GetComponent<RectTransform>()));
            
            // Wait for the last one to finish plus the pause
            yield return new WaitForSeconds(bounceDuration + pauseDuration);
        }
    }

    private IEnumerator SingleBounce(RectTransform crystal)
    {
        if (crystal == null) yield break;

        Vector2 startPos = crystal.anchoredPosition;
        float startY = startPos.y;

        // Jump Up and Fall Down combined
        float elapsed = 0f;
        while (elapsed < bounceDuration)
        {
            // Use UN-SCALED time: This makes the bounce independent of loading lag/pauses.
            // Even if the main thread hitches for 0.1s, the crystal will "jump" to the correct position.
            elapsed += Time.unscaledDeltaTime;
            float normalizedProgress = elapsed / bounceDuration;
            
            // Simple curve: sin(pi * progress) goes 0 -> 1 -> 0
            float height = Mathf.Sin(normalizedProgress * Mathf.PI) * bounceHeight;
            
            if (crystal != null)
                crystal.anchoredPosition = new Vector2(startPos.x, startY + height);
            
            yield return null;
        }
        
        // Final position cleanup
        if (crystal != null)
            crystal.anchoredPosition = new Vector2(startPos.x, startY);
    }
}