using UnityEngine;
using TMPro;
using System.Collections;

public class FishTooltip : MonoBehaviour
{
    public static FishTooltip Instance;
    
    [Tooltip("Drag the TextMeshPro text that will show the word here")]
    public TextMeshProUGUI wordText;
    
    [Tooltip("How far above the fish the tooltip should float.")]
    public Vector3 offset = new Vector3(0, 50f, 0); 
    
    [Tooltip("How fast the tooltip catches up to the fish as it swims")]
    public float smoothSpeed = 15f;
    
    [Header("Animation")]
    [Tooltip("How fast the tooltip pops open and closed")]
    public float popAnimationSpeed = 15f; 
    
    private FishController targetFish;
    private FishController pendingFish;   // The next fish to show, waiting for the hide to finish
    private string pendingWord = "";
    private bool isShowing = false;
    private Vector3 targetScale = Vector3.zero;
    private bool isSwitching = false;     // True while we are hiding before switching to a new fish

    void Awake()
    {
        Instance = this;
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false); 
    }

    public void ShowTooltip(FishController fish, string word)
    {
        // If we click the SAME fish that is already showing, toggle it off
        if (targetFish == fish && isShowing && !isSwitching)
        {
            HideTooltip();
            return;
        }

        // If the tooltip is currently visible, shrink it first then pop up on the new fish
        if (isShowing || isSwitching)
        {
            pendingFish = fish;
            pendingWord = word;
            isSwitching = true;
            targetScale = Vector3.zero; // Start shrinking
        }
        else
        {
            // It was already hidden, just pop straight up on the new fish
            OpenOnFish(fish, word);
        }
    }

    void OpenOnFish(FishController fish, string word)
    {
        targetFish = fish;
        pendingFish = null;
        isSwitching = false;

        if (wordText != null) wordText.text = word;

        gameObject.SetActive(true);
        isShowing = true;
        targetScale = Vector3.one;

        // Snap position so it doesn't fly from across the screen
        transform.position = targetFish.transform.position + offset;
        transform.localScale = Vector3.zero; // Start from tiny so it pops open
    }

    public void HideTooltip()
    {
        isShowing = false;
        isSwitching = false;
        pendingFish = null;
        targetScale = Vector3.zero;
    }

    void Update()
    {
        // Smoothly animate the scale
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * popAnimationSpeed);

        // Check if we finished shrinking
        if (transform.localScale.x < 0.01f)
        {
            if (isSwitching && pendingFish != null)
            {
                // We finished hiding — now pop open on the new fish!
                OpenOnFish(pendingFish, pendingWord);
            }
            else if (!isShowing)
            {
                // Fully hidden, turn off to save performance
                gameObject.SetActive(false);
                targetFish = null;
            }
        }

        // Smoothly follow the fish
        if (targetFish != null && isShowing)
        {
            Vector3 targetPos = targetFish.transform.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);
        }
    }
}
