using UnityEngine;

public class UIBreathingEffect : MonoBehaviour
{
    [Header("Breathing Settings")]
    [Tooltip("How large it grows (1.02 means 2% larger)")]
    public float scaleAmount = 1.02f; 
    [Tooltip("How fast it pulses")]
    public float speed = 1.5f;        

    private Vector3 _originalScale;

    void Start()
    {
        _originalScale = transform.localScale;
    }

    void Update()
    {
        // Use a sine wave to oscillate between 1 and scaleAmount
        float wave = (Mathf.Sin(Time.time * speed) + 1f) / 2f; 
        float currentScale = Mathf.Lerp(1f, scaleAmount, wave);
        transform.localScale = _originalScale * currentScale;
    }
}
