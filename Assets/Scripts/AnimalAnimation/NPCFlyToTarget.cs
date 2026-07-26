using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Smoothly moves a flying NPC (like Flowerpecker / Tiptip) from its initial starting position (Pos A)
/// to a target position (Pos B) when triggered.
/// Perfect for cutscenes, arrival events, and quest area triggers.
/// </summary>
public class NPCFlyToTarget : MonoBehaviour
{
    [Header("Flight Target Settings")]
    [Tooltip("The destination Transform where the NPC should fly down to.")]
    public Transform targetPoint;

    [Tooltip("Duration of the flight in seconds.")]
    public float flyDuration = 2.0f;

    [Tooltip("Smooth easing curve for the flight path.")]
    public AnimationCurve flightCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Tooltip("If true, rotates the NPC toward the target while flying.")]
    public bool rotateTowardFlightDirection = true;

    [Tooltip("If true, rotates the NPC to face the player after landing.")]
    public bool facePlayerOnArrival = true;

    [Header("Arrival Events")]
    [Tooltip("Fires automatically when the NPC finishes flying to the target point.")]
    public UnityEvent OnArrival;

    private bool _isFlying = false;

    /// <summary>
    /// Call this method to start the smooth fly-down sequence.
    /// Can be hooked up directly in ProximityTrigger's OnTriggered event.
    /// </summary>
    public void FlyToTarget()
    {
        if (_isFlying) return;
        if (targetPoint == null)
        {
            Debug.LogWarning($"[NPCFlyToTarget] No Target Point assigned on {gameObject.name}!");
            OnArrival?.Invoke();
            return;
        }

        StartCoroutine(FlyRoutine());
    }

    private IEnumerator FlyRoutine()
    {
        _isFlying = true;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Vector3 endPos = targetPoint.position;
        Quaternion endRot = targetPoint.rotation;

        float elapsed = 0f;

        while (elapsed < flyDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / flyDuration);
            float curveT = flightCurve.Evaluate(t);

            // Interpolate position
            transform.position = Vector3.Lerp(startPos, endPos, curveT);

            // Rotate toward direction of flight
            if (rotateTowardFlightDirection && (endPos - startPos).sqrMagnitude > 0.01f)
            {
                Vector3 moveDir = (endPos - startPos).normalized;
                Quaternion lookDir = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(startRot, lookDir, t * 2f);
            }

            yield return null;
        }

        // Snap to exact end position
        transform.position = endPos;

        // Face player if requested
        if (facePlayerOnArrival)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Vector3 lookAtPos = player.transform.position;
                lookAtPos.y = transform.position.y; // Keep level horizon
                transform.rotation = Quaternion.LookRotation((lookAtPos - transform.position).normalized, Vector3.up);
            }
            else
            {
                transform.rotation = endRot;
            }
        }
        else
        {
            transform.rotation = endRot;
        }

        _isFlying = false;
        Debug.Log($"[NPCFlyToTarget] {gameObject.name} arrived at target point.");
        OnArrival?.Invoke();
    }
}
