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
        if (_playerTransform == null || talkButton == null) return;

        // Hide button during dialogue or if HUD is suppressed (Lessons, Mini-games, etc.)
        bool isHUDSuppressed = HUDManager.Instance != null && !HUDManager.Instance.IsHUDAllowed;
        
        if ((DialogueManager.Instance != null && DialogueManager.Instance.IsInDialogue) || isHUDSuppressed)
        {
            talkButton.gameObject.SetActive(false);
            _currentNearest = null;
            return;
        }

        InteractableBase nearest = null;
        float shortestDistance = float.MaxValue;

        // Find the closest interactable
        foreach (var interactable in _allInteractables)
        {
            if (interactable == null || !interactable.isActiveAndEnabled || !interactable.interactionEnabled) continue;

            float dist = Vector3.Distance(_playerTransform.position, interactable.transform.position);
            
            if (dist <= interactable.interactionDistance && dist < shortestDistance)
            {
                shortestDistance = dist;
                nearest = interactable;
            }
        }

        // If the nearest interactable changed
        if (nearest != _currentNearest)
        {
            _currentNearest = nearest;

        // Final visibility decision
        bool isNearInteractable = _currentNearest != null;
        bool shouldShowButton = isNearInteractable && !isHUDSuppressed;

        if (talkButton.gameObject.activeSelf != shouldShowButton)
        {
            talkButton.gameObject.SetActive(shouldShowButton);
            
            if (shouldShowButton && buttonText != null)
            {
                buttonText.text = _currentNearest.promptText;
            }
        }
        }
    }

    private void OnButtonClicked()
    {
        if (_currentNearest != null)
        {
            // Hide the button so they can't spam it during dialogue
            talkButton.gameObject.SetActive(false);
            
            // Tell the interactable to do its thing
            _currentNearest.Interact();
            
            // We temporarily clear nearest so the button stays hidden 
            // until ResetInteraction() is called or we walk away and come back.
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
