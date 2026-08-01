using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Global manager that controls the Talk Button UI.
/// It automatically detects nearby InteractableNPCs and shows the button.
/// Attach this to an empty GameObject in your scene or directly to your Canvas.
/// </summary>
public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }

    [Header("UI References")]
    [Tooltip("The shared Talk Button in your Canvas.")]
    public Button talkButton;

    [Tooltip("The Text component inside the Talk Button.")]
    public TextMeshProUGUI buttonText;

    [Header("Player Settings")]
    [Tooltip("Tag of your player character.")]
    public string playerTag = "Player";

    private Transform _playerTransform;
    
    // Keeps track of all interactables in the scene to avoid expensive FindObjects calls
    private List<InteractableBase> _allInteractables = new List<InteractableBase>();
    
    // The interactable we are currently closest to
    private InteractableBase _currentNearest = null;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            _playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("[InteractionManager] Could not find Player. Check your Player Tag.");
        }

        if (talkButton != null)
        {
            // Bind the button click to our central handler
            talkButton.onClick.AddListener(OnButtonClicked);
            talkButton.gameObject.SetActive(false); // Hide initially
        }
        else
        {
            Debug.LogWarning("[InteractionManager] Talk Button is not assigned!");
        }
    }

    void Update()
    {
        if (talkButton == null) return;


        // Professional Player Detection: Find the object with the CharacterController (the real mover)
        if (_playerTransform == null || !_playerTransform.gameObject.activeInHierarchy)
        {
            var controllers = GameObject.FindObjectsByType<CharacterController>(FindObjectsSortMode.None);
            foreach (var cc in controllers)
            {
                if (cc.gameObject.CompareTag(playerTag) && cc.gameObject.activeInHierarchy)
                {
                    _playerTransform = cc.transform;
                    break;
                }
            }
            if (_playerTransform == null) return; 
        }

        // Hide button during dialogue or if HUD is suppressed
        bool isHUDSuppressed = HUDManager.Instance != null && !HUDManager.Instance.IsHUDAllowed;
        bool isInDialogue = DialogueManager.Instance != null && DialogueManager.Instance.IsInDialogue;
        
        if (isInDialogue || isHUDSuppressed)
        {
            if (talkButton.gameObject.activeSelf) talkButton.gameObject.SetActive(false);
            _currentNearest = null;
            return;
        }

        InteractableBase nearest        = null;
        InteractableBase nearestInGrace  = null;  // within 1.5x grace zone (keeps button visible)
        float shortestDistance           = float.MaxValue;
        float shortestGraceDistance      = float.MaxValue;

        foreach (var interactable in _allInteractables)
        {
            if (interactable == null || !interactable.isActiveAndEnabled || !interactable.interactionEnabled) continue;

            float dist = Vector3.Distance(_playerTransform.position, interactable.transform.position);

            if (dist <= interactable.interactionDistance && dist < shortestDistance)
            {
                shortestDistance = dist;
                nearest = interactable;
            }

            // Keep button showing if player is within 1.5x of interaction distance
            float graceDist = interactable.interactionDistance * 1.5f;
            if (dist <= graceDist && dist < shortestGraceDistance)
            {
                shortestGraceDistance = dist;
                nearestInGrace = interactable;
            }
        }

        // Lock nearest: prefer the true in-range target; fall back to grace-zone
        // so the button stays visible while the player is "close enough" to click.
        InteractableBase effective = nearest ?? nearestInGrace;

        // Log distance to console to see what's happening
        if (Time.frameCount % 120 == 0 && nearest != null)
        {
            Debug.Log($"[InteractionManager] Nearest: {nearest.gameObject.name}, Distance: {shortestDistance:F2}");
        }

        if (effective != _currentNearest)
        {
            _currentNearest = effective;
        }

        bool shouldShowButton = _currentNearest != null && !isHUDSuppressed;

        if (talkButton.gameObject.activeSelf != shouldShowButton)
        {
            talkButton.gameObject.SetActive(shouldShowButton);

            if (shouldShowButton && buttonText != null && _currentNearest != null)
            {
                buttonText.text = _currentNearest.promptText;
            }
        }

        // --- DEBUG FALLBACK ---
        // If UI clicks are being swallowed by an invisible panel,
        // pressing E or Enter will bypass the UI and force the interaction.
#if ENABLE_INPUT_SYSTEM
        if (shouldShowButton && UnityEngine.InputSystem.Keyboard.current != null)
        {
            if (UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame || UnityEngine.InputSystem.Keyboard.current.enterKey.wasPressedThisFrame)
            {
                Debug.Log("[InteractionManager] Forced interaction via keyboard!");
                OnButtonClicked();
            }
        }
#else
        if (shouldShowButton && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return)))
        {
            Debug.Log("[InteractionManager] Forced interaction via keyboard!");
            OnButtonClicked();
        }
#endif
    }

    private void OnButtonClicked()
    {
        // If _currentNearest was cleared by this frame's Update before the click fired,
        // do a fresh scan with generous grace distance so the click always works.
        if (_currentNearest == null && _playerTransform != null)
        {
            float bestDist = float.MaxValue;
            foreach (var interactable in _allInteractables)
            {
                if (interactable == null || !interactable.isActiveAndEnabled || !interactable.interactionEnabled) continue;
                float dist = Vector3.Distance(_playerTransform.position, interactable.transform.position);
                float graceDist = interactable.interactionDistance * 2f;
                if (dist <= graceDist && dist < bestDist)
                {
                    bestDist = dist;
                    _currentNearest = interactable;
                }
            }
        }

        Debug.Log($"[InteractionManager] OnButtonClicked! _currentNearest is {(_currentNearest != null ? _currentNearest.gameObject.name : "NULL")}");
        if (_currentNearest != null)
        {
            talkButton.gameObject.SetActive(false);
            _currentNearest.Interact();
            _currentNearest = null;
        }
    }

    // ── Public API for Interactables ────────────────────────────────────────

    public void RegisterInteractable(InteractableBase i)
    {
        if (!_allInteractables.Contains(i))
            _allInteractables.Add(i);
    }

    public void UnregisterInteractable(InteractableBase i)
    {
        if (_allInteractables.Contains(i))
            _allInteractables.Remove(i);
    }
    
    /// <summary>
    /// Call this when dialogue finishes so the button can appear again
    /// if the player is still standing there.
    /// </summary>
    public void ForceCheckProximity()
    {
        // Forces the Update loop to re-evaluate nearest interactable next frame
        _currentNearest = null; 
    }
}
