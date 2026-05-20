using UnityEngine;
using TMPro;

public class FloatingSpeechBubble : MonoBehaviour
{
    [Header("References")]
    public GameObject bubbleContainer;
    public TextMeshProUGUI speechText;
    public Transform targetNPC;
    public Vector3 offset = new Vector3(0, 2.5f, 0);

    [Header("Quest Sync")]
    public string requiredObjective;
    public string textToShow = "...";

    private CanvasGroup _canvasGroup;
    private bool _isVisible = false;

    void Awake()
    {
        if (bubbleContainer != null)
            _canvasGroup = bubbleContainer.GetComponent<CanvasGroup>();
        
        if (bubbleContainer != null)
            bubbleContainer.SetActive(false);
    }

    void Update()
    {
        if (ObjectiveManager.Instance == null || string.IsNullOrEmpty(requiredObjective)) return;

        bool shouldShow = ObjectiveManager.Instance.CurrentObjective.StartsWith(requiredObjective, System.StringComparison.OrdinalIgnoreCase);

        if (shouldShow && !_isVisible)
        {
            ShowBubble();
        }
        else if (!shouldShow && _isVisible)
        {
            HideBubble();
        }

        if (_isVisible && targetNPC != null)
        {
            // Position the bubble above the NPC
            transform.position = targetNPC.position + offset;
            
            // Make it face the camera
            if (Camera.main != null)
            {
                transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
            }
        }
    }

    public void ShowBubble()
    {
        if (bubbleContainer == null) return;
        _isVisible = true;
        bubbleContainer.SetActive(true);
        if (speechText != null) speechText.text = textToShow;
    }

    public void HideBubble()
    {
        if (bubbleContainer == null) return;
        _isVisible = false;
        bubbleContainer.SetActive(false);
    }
}
