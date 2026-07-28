using UnityEngine;

/// <summary>
/// Smoothly bobs a GameObject (like Flowerpecker / Tiptip) up and down while playing its flying animation.
/// Uses a sine wave offset so it works seamlessly during idle, hovering, and movement!
/// </summary>
public class HoverAnimation : MonoBehaviour
{
    [Header("Hover Settings")]
    [Tooltip("How high up and down the bird bobs (in meters).")]
    public float hoverHeight = 0.25f;

    [Tooltip("How fast the bird oscillates up and down.")]
    public float hoverSpeed = 2.5f;

    [Tooltip("Set to false to pause hovering bobbing.")]
    public bool enableHover = true;

    // ── Private state ───────────
    private Vector3 _basePosition;
    private float _randomOffset;

    private void Start()
    {
        _basePosition = transform.position;
        // Randomize phase so multiple birds don't bob in exact sync
        _randomOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    private void LateUpdate()
    {
        if (!enableHover) return;

        // Calculate sine wave bobbing offset only on Y
        float yOffset = Mathf.Sin((Time.time * hoverSpeed) + _randomOffset) * hoverHeight;

        // Only adjust Y — don't touch X/Z so other movement scripts aren't overridden
        transform.position = new Vector3(
            transform.position.x,
            _basePosition.y + yOffset,
            transform.position.z
        );
    }

    /// <summary>
    /// Update base position when the bird is moved via script or teleported.
    /// </summary>
    public void SetBasePosition(Vector3 newBasePos)
    {
        _basePosition = newBasePos;
    }
}
