using UnityEngine;

[ExecuteInEditMode]
public class CameraAutoAlign : MonoBehaviour
{
    [Tooltip("Drag the PlayerArmature / Girl object here")]
    public Transform girl;

    [Header("Adjustments")]
    public float cameraHeight = 1.45f;
    public float lookAtHeight = 1.45f;
    public float distance = 0.8f;

    void Update()
    {
        if (girl == null) return;

        // 1. Calculate the target point (the face)
        Vector3 targetPoint = girl.position + Vector3.up * lookAtHeight;

        // 2. Position the camera relative to the girl's position but with its own height
        Vector3 offset = girl.forward * distance;
        transform.position = girl.position + offset + (Vector3.up * cameraHeight);
        
        // 3. Look at the target point
        transform.LookAt(targetPoint);
    }
}
