using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class FishListItem : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("Drag the child Image that shows the fish here")]
    public Image fishIcon;
    
    [Tooltip("Drag the child TextMeshPro object here")]
    public TextMeshProUGUI wordText;

    [Header("Highlight Settings")]
    [Tooltip("The background image that will change color (leave empty to automatically use this object's Image)")]
    public Image backgroundImage;
    
    [Tooltip("The color it changes to when clicked!")]
    public Color highlightColor = new Color(1f, 0.92f, 0.3f); // A soft nice yellow
    
    [Tooltip("How fast the color smoothly transitions")]
    public float colorFadeSpeed = 15f;
    
    private Color originalColor;
    private Color targetColor;
    
    // This 'static' variable is shared across ALL list items. It remembers which one is currently yellow!
    private static FishListItem currentlySelected;

    void Awake()
    {
        // Automatically grab the background image component and remember your perfect beige color!
        if (backgroundImage == null) backgroundImage = GetComponent<Image>();
        if (backgroundImage != null) 
        {
            originalColor = backgroundImage.color;
            targetColor = originalColor; // Start by aiming for the normal color
        }
    }

    public void Setup(Sprite fishSprite, string word)
    {
        if (fishIcon != null && fishSprite != null) 
        {
            fishIcon.sprite = fishSprite;
        }
        
        if (wordText != null) 
        {
            wordText.text = word;
        }
    }

    void Update()
    {
        // Smoothly blend the current color towards our target color every frame
        if (backgroundImage != null)
        {
            backgroundImage.color = Color.Lerp(backgroundImage.color, targetColor, Time.deltaTime * colorFadeSpeed);
        }
    }

    // This runs when you click the word box
    public void OnPointerClick(PointerEventData eventData)
    {
        // If we click the one that's already yellow, tell it to fade back to beige
        if (currentlySelected == this)
        {
            targetColor = originalColor;
            currentlySelected = null;
            return;
        }

        // Tell the previously clicked one to fade back to beige so only ONE can be yellow at a time!
        if (currentlySelected != null)
        {
            currentlySelected.targetColor = currentlySelected.originalColor;
        }

        // Tell this one to fade to yellow!
        targetColor = highlightColor;

        // Remember that this one is now the selected one
        currentlySelected = this;
    }
}
