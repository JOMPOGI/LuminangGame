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
    
    // Keeps track of all NPCs in the scene to avoid expensive FindObjects calls
    private List<InteractableNPC> _allInteractables = new List<InteractableNPC>();
    
    // The NPC we are currently closest to
    private InteractableNPC _currentNearestNPC = null;

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
            talkButton.onClick.AddListener(OnTalkButtonClicked);
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

        // Safety: If dialogue is currently showing, hide the proximity talk button
        if (DialogueManager.Instance != null && 
            DialogueManager.Instance.uiController != null && 
            DialogueManager.Instance.uiController.dialoguePanel.activeInHierarchy)
        {
            talkButton.gameObject.SetActive(false);
            _currentNearestNPC = null; // Reset so it re-detects when dialogue ends
            return;
        }

        InteractableNPC nearestNPC = null;
        float shortestDistance = float.MaxValue;

        // Find the closest interactable NPC
        foreach (var npc in _allInteractables)
        {
            if (npc == null || !npc.isActiveAndEnabled) continue;

            float dist = Vector3.Distance(_playerTransform.position, npc.transform.position);
            
            if (dist <= npc.interactionDistance && dist < shortestDistance)
            {
                shortestDistance = dist;
                nearestNPC = npc;
            }
        }

        // If the nearest NPC changed
        if (nearestNPC != _currentNearestNPC)
        {
            _currentNearestNPC = nearestNPC;

            if (_currentNearestNPC != null)
            {
                // We got near someone! Show button and update its text.
                talkButton.gameObject.SetActive(true);
                
                if (buttonText != null)
                {
                    buttonText.text = _currentNearestNPC.promptText;
                }
            }
            else
            {
                // We walked away from everyone. Hide button.
                talkButton.gameObject.SetActive(false);
            }
        }
    }

    private void OnTalkButtonClicked()
    {
        if (_currentNearestNPC != null)
        {
            // Hide the button so they can't spam it during dialogue
            talkButton.gameObject.SetActive(false);
            
            // Tell the NPC to do its thing
            _currentNearestNPC.Interact();
            
            // We temporarily clear nearest NPC so the button stays hidden 
            // until ResetInteraction() is called or we walk away and come back.
            _currentNearestNPC = null;
        }
    }

    // ── Public API for NPCs ────────────────────────────────────────

    public void RegisterNPC(InteractableNPC npc)
    {
        if (!_allInteractables.Contains(npc))
            _allInteractables.Add(npc);
    }

    public void UnregisterNPC(InteractableNPC npc)
    {
        if (_allInteractables.Contains(npc))
            _allInteractables.Remove(npc);
    }
    
    /// <summary>
    /// Call this when dialogue finishes so the button can appear again
    /// if the player is still standing there.
    /// </summary>
    public void ForceCheckProximity()
    {
        // Forces the Update loop to re-evaluate nearest NPC next frame
        _currentNearestNPC = null; 
    }
}
