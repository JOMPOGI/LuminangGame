using UnityEngine;

/// <summary>
/// Rotates a wheel mesh based on the actual distance moved by its parent.
/// Handles forward/backward rolling and stops when stationary.
/// </summary>
public class WheelRotator : MonoBehaviour
{
    [Header("Wheel Specs")]
    [Tooltip("Approximate radius of the wheel in Unity units. Used to calculate correct rotation speed.")]
    public float wheelRadius = 0.5f;

    [Tooltip("The local axis the wheel rotates around. Usually local Z or local X depending on the FBX export.")]
    public Vector3 rotationAxis = Vector3.forward; // (0, 0, 1) represents local Z

    private Vector3 _lastParentPosition;
    private Transform _parentTransform;

    void Start()
    {
        // We track the parent's movement (e.g. the Horse or the Wagon root)
        _parentTransform = transform.parent;
        if (_parentTransform != null)
        {
            _lastParentPosition = _parentTransform.position;
        }
    }

    void Update()
    {
        if (_parentTransform == null) return;

        // Calculate distance moved along the ground (XZ plane) since last frame
        Vector3 currentParentPos = _parentTransform.position;
        Vector3 movement = currentParentPos - _lastParentPosition;
        movement.y = 0; // Ignore vertical changes
        
        float distanceMoved = movement.magnitude;

        if (distanceMoved > 0.0001f)
        {
            // Determine direction of rotation (forward or backward relative to the parent's forward axis)
            float direction = Vector3.Dot(movement.normalized, _parentTransform.forward) >= 0 ? 1f : -1f;

            // Calculate rotation angle in degrees: (Distance / Circumference) * 360
            float rotationAngle = (distanceMoved / (2f * Mathf.PI * wheelRadius)) * 360f * direction;

            // Rotate around the specified local axis
            transform.Rotate(rotationAxis, rotationAngle, Space.Self);
        }

        _lastParentPosition = currentParentPos;
    }
}
