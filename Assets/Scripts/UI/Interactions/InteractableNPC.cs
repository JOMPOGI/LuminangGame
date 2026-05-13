using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class InteractableNPC : InteractableBase
{
    [Header("Dialogue Settings")]
    [Tooltip("The casual dialogue used when the NPC has nothing specific to do with the current quest.")]
    public DialogueNode defaultDialogue;

    [Header("Quest Integration")]
    [Tooltip("Check this if this NPC is one of the targets for a scavenger hunt/greeting quest.")]
    public bool isOrganizer = false;
    private bool _hasBeenGreeted = false;

    [System.Serializable]
    public class QuestDialogue
    {
        [Tooltip("The objective that must be active for this dialogue to trigger.")]
        public string requiredObjective;
        [Tooltip("The dialogue to play during this specific quest stage.")]
        public DialogueNode dialogueNode;
    }

    [Tooltip("List of special dialogues that only trigger during specific quest objectives.")]
    public List<QuestDialogue> questDialogues = new List<QuestDialogue>();

    public Animator npcAnimator;

    [Header("One-Time Interaction")]
    [Tooltip("If true, the interaction button will NEVER appear again after the first conversation ends.")]
    public bool disableAfterInteraction = false;
    [HideInInspector] public bool isWrongAnswerPlaying = false;

    [Header("Events")]
    public UnityEvent OnDialogueEnd;
    public UnityEvent OnWrongAnswer;

    public override void Interact()
    {
        if (!interactionEnabled) return;

        DialogueNode nodeToPlay = GetCurrentDialogueNode();
        ForceStartDialogue(nodeToPlay);

        OnInteract?.Invoke();
    }

    /// <summary>
    /// Manually triggers a specific dialogue node on this NPC.
    /// Great for location triggers or cutscenes!
    /// </summary>
    public void ForceStartDialogue(DialogueNode node)
    {
        if (node != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(node, npcAnimator, this);
        }
    }

    private DialogueNode GetCurrentDialogueNode()
    {
        if (ObjectiveManager.Instance != null && questDialogues != null)
        {
            string currentObj = ObjectiveManager.Instance.CurrentObjective;
            foreach (var qd in questDialogues)
            {
                if (currentObj != null && currentObj.StartsWith(qd.requiredObjective, System.StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log($"[{gameObject.name}] Match found! Using Quest Dialogue: {qd.dialogueNode.name}");
                    return qd.dialogueNode;
                }
            }
        }

        // 2. Fallback to default dialogue
        return defaultDialogue;
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

    /// <summary>
    /// Helper method to update the player's objective. 
    /// Easily callable from UnityEvents (like OnDialogueEnd).
    /// </summary>
    public void SetNewObjective(string objective)
    {
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.SetObjective(objective);
        }
    }

    [Header("Custom Scene Events")]
    [Tooltip("Map event strings from Dialogue Nodes to Unity Events in the scene.")]
    public List<DialogueEventMapping> dialogueEvents = new List<DialogueEventMapping>();

    public void TriggerWrongAnswerAnimation()
    {
        if (npcAnimator != null)
        {
            StartCoroutine(WrongAnswerRoutine());
        }
    }

    private IEnumerator WrongAnswerRoutine()
    {
        isWrongAnswerPlaying = true;
        
        // Let the UnityEvent fire (which likely triggers the Animator)
        OnWrongAnswer?.Invoke();

        // Wait a moment for the animator to transition
        yield return new WaitForSeconds(0.2f);

        // Wait while the animator is in ANY state other than the base Idle
        // This assumes the wrong answer animation is NOT the default state.
        if (npcAnimator != null)
        {
            float elapsed = 0f;
            while (elapsed < 5f) // Safety timeout
            {
                var state = npcAnimator.GetCurrentAnimatorStateInfo(0);
                // If we've returned to the Idle state (assuming it's named "Idle" or contains "Idle")
                if (state.IsName("apoLakay_Idle") || state.IsName("Idle")) 
                    break;
                
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        isWrongAnswerPlaying = false;
    }

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

    /// <summary>
    /// Smoothly rotates the NPC to face the player.
    /// Can be called from the OnInteract event.
    /// </summary>
    public void SmoothLookAtPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            StartCoroutine(LookAtRoutine(player.transform));
        }
    }

    /// <summary>
    /// Helper to trigger the lesson panel with a specific category.
    /// </summary>
    public void StartLessonWithCategory(string category)
    {
        if (LessonManager.Instance != null)
        {
            LessonManager.Instance.ShowLessonWithCategory(category);
        }
    }

    /// <summary>
    /// Helper to trigger a minigame. Drag a prefab into the UnityEvent slot!
    /// </summary>
    public void StartMinigame(GameObject minigamePrefab)
    {
        StartMinigameWithCategory(minigamePrefab, "");
    }

    /// <summary>
    /// Helper to trigger a minigame with a specific category tag.
    /// Useful for dynamic minigames that load content based on the lesson.
    /// </summary>
    public void StartMinigameWithCategory(GameObject minigamePrefab, string category)
    {
        if (MinigameManager.Instance != null)
        {
            MinigameManager.Instance.StartMinigameWithCategory(minigamePrefab, category);
        }
    }

    /// <summary>
    /// Call this via Dialogue Events to progress the 'Identify Organizers' quest.
    /// Only works if 'isOrganizer' is checked and they haven't been greeted yet.
    /// </summary>
    public void GreetOrganizer()
    {
        Debug.Log($"[{gameObject.name}] GreetOrganizer called! isOrganizer: {isOrganizer}, hasBeenGreeted: {_hasBeenGreeted}");
        if (isOrganizer && !_hasBeenGreeted)
        {
            _hasBeenGreeted = true;
            Debug.Log($"[{gameObject.name}] Success! Adding progress to ObjectiveManager.");
            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.AddProgress();
            }
        }
    }

    private IEnumerator LookAtRoutine(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0; // Keep the NPC upright
        
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion startRot = transform.rotation;
            Quaternion targetRot = Quaternion.LookRotation(direction);
            
            float elapsed = 0f;
            float duration = 0.6f; // Time it takes to turn

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsed / duration);
                transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
                yield return null;
            }
            transform.rotation = targetRot;
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
