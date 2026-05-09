using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class InteractableNPC : InteractableBase
{
    [Header("Dialogue Settings")]
    public DialogueNode startingDialogueNode;
    public Animator npcAnimator;

    [Header("One-Time Interaction")]
    [Tooltip("If true, the interaction button will NEVER appear again after the first conversation ends.")]
    public bool disableAfterInteraction = false;

    [Header("Events")]
    public UnityEvent OnDialogueEnd;
    public UnityEvent OnWrongAnswer;

    [HideInInspector] public bool isWrongAnswerPlaying = false;

    public override void Interact()
    {
        if (!interactionEnabled) return;

        if (startingDialogueNode != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(startingDialogueNode, npcAnimator, this);
        }

        OnInteract?.Invoke();
    }

    public void EnableInteraction() => interactionEnabled = true;
    public void DisableInteraction() => interactionEnabled = false;

    /// <summary>
    /// Helper method to teleport the NPC. Easily callable from UnityEvents.
    /// </summary>
    public void TeleportTo(Transform targetTransform)
    {
        if (targetTransform != null)
        {
            transform.position = targetTransform.position;
            transform.rotation = targetTransform.rotation;
        }
    }

    /// <summary>
    /// Forces the player's third-person camera to immediately snap and look at this NPC.
    /// Easily callable from UnityEvents.
    /// </summary>
    public void ForcePlayerCameraToLookAtMe()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var tpc = player.GetComponent<StarterAssets.ThirdPersonController>();
            if (tpc != null)
            {
                tpc.ForceCameraLookAt(transform.position);
            }
        }
    }

    /// <summary>
    /// Safely hides the player's 3D model without breaking their physics or controller.
    /// Useful for dialogue close-ups! Easily callable from UnityEvents.
    /// </summary>
    public void HidePlayer()
    {
        SetPlayerVisibility(false);
    }

    /// <summary>
    /// Shows the player's 3D model again. Easily callable from UnityEvents.
    /// </summary>
    public void ShowPlayer()
    {
        SetPlayerVisibility(true);
    }

    private void SetPlayerVisibility(bool isVisible)
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            // Find the child object that usually holds the armature/mesh in StarterAssets
            // Usually we just disable all SkinnedMeshRenderers or MeshRenderers
            var renderers = player.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                r.enabled = isVisible;
            }
        }
    }

    private Coroutine _cameraCoroutine;
    private Vector3 _originalCamPos;
    private Quaternion _originalCamRot;

    /// <summary>
    /// Foolproof method to smoothly transition the main camera to a specific close-up spot.
    /// It automatically disables Cinemachine temporarily so you don't have to mess with priorities!
    /// </summary>
    public void EnterCloseUp(Transform closeUpSpot)
    {
        if (closeUpSpot == null) return;
        
        HidePlayer(); // Automatically hide the player
        
        GameObject mainCam = GameObject.FindWithTag("MainCamera");
        if (mainCam != null)
        {
            // Support both Cinemachine 2 and 3 namespaces
            Behaviour brain = mainCam.GetComponent("CinemachineBrain") as Behaviour;
            if (brain != null) brain.enabled = false;

            if (_cameraCoroutine != null) StopCoroutine(_cameraCoroutine);
            _cameraCoroutine = StartCoroutine(LerpCamera(mainCam.transform, closeUpSpot.position, closeUpSpot.rotation, 1f));
        }
    }

    /// <summary>
    /// Smoothly transitions the camera back to normal gameplay.
    /// </summary>
    public void ExitCloseUp()
    {
        ShowPlayer(); // Bring player back
        
        GameObject mainCam = GameObject.FindWithTag("MainCamera");
        if (mainCam != null)
        {
            Behaviour brain = mainCam.GetComponent("CinemachineBrain") as Behaviour;
            if (brain != null) 
            {
                brain.enabled = true; // Cinemachine will automatically smooth-blend back!
            }
            else if (_originalCamPos != Vector3.zero) 
            {
                 mainCam.transform.position = _originalCamPos;
                 mainCam.transform.rotation = _originalCamRot;
            }
        }
    }

    private System.Collections.IEnumerator LerpCamera(Transform cam, Vector3 targetPos, Quaternion targetRot, float duration)
    {
        _originalCamPos = cam.position;
        _originalCamRot = cam.rotation;

        Vector3 startPos = cam.position;
        Quaternion startRot = cam.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // Smooth ease in/out
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            cam.position = Vector3.Lerp(startPos, targetPos, t);
            cam.rotation = Quaternion.Lerp(startRot, targetRot, t);
            yield return null;
        }
        cam.position = targetPos;
        cam.rotation = targetRot;
    }

    [Header("Custom Scene Events")]
    [Tooltip("Map event strings from Dialogue Nodes to Unity Events in the scene.")]
    public List<DialogueEventMapping> dialogueEvents = new List<DialogueEventMapping>();

    public void HandleDialogueEvent(string eventName)
    {
        if (string.IsNullOrEmpty(eventName)) return;
        
        string cleanEventName = eventName.Trim();
        Debug.Log($"[InteractableNPC] Received Dialogue Event: '{cleanEventName}' on NPC: {gameObject.name}");

        foreach (var mapping in dialogueEvents)
        {
            if (mapping.eventName != null && mapping.eventName.Trim() == cleanEventName)
            {
                Debug.Log($"[InteractableNPC] Match found! Firing UnityEvents for: '{cleanEventName}'");
                mapping.onEventTriggered?.Invoke();
            }
        }
    }
}

[System.Serializable]
public class DialogueEventMapping
{
    [Tooltip("The string defined in the Dialogue Node's 'Trigger Event Name' field.")]
    public string eventName;
    public UnityEvent onEventTriggered;
}
