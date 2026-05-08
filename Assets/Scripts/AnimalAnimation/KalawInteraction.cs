using UnityEngine;

/// <summary>
/// Helper script for the Kalaw bird.
/// Now that UI is handled globally, this script just serves as a way
/// to restart the bird's animation when a conversation finishes.
/// </summary>
public class KalawInteraction : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The KalawIdleTest script on this same GameObject.")]
    public KalawIdleTest kalawIdleTest;

    /// <summary>
    /// Call this to reset the interaction (e.g., after dialogue ends)
    /// so the player can talk again and the bird starts moving again.
    /// </summary>
    public void ResetInteraction()
    {
        if (kalawIdleTest != null)
        {
            kalawIdleTest.ResumeIdleSystem();
        }
        
        // Force the InteractionManager to re-check if we are still standing near the bird
        if (InteractionManager.Instance != null)
        {
            InteractionManager.Instance.ForceCheckProximity();
        }
    }
}

