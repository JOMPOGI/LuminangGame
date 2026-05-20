using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A simple bridge script to allow Prefabs to end minigames.
/// Attach this to a Button in your Minigame Prefab!
/// </summary>
[RequireComponent(typeof(Button))]
public class MinigameEndTrigger : MonoBehaviour
{
    void Start()
    {
        // Automatically hook up the click event
        GetComponent<Button>().onClick.AddListener(EndMinigame);
    }

    public void EndMinigame()
    {
        if (MinigameManager.Instance != null)
        {
            MinigameManager.Instance.HideMinigame();
        }
        else
        {
            Debug.LogWarning("[MinigameEndTrigger] Could not find MinigameManager instance!");
        }
    }
}
