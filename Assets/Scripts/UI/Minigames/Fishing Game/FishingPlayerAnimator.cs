using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FishingPlayerAnimator : MonoBehaviour
{
    [Tooltip("Drag the Player's SpriteRenderer here if it's a 2D World object")]
    public SpriteRenderer spriteRenderer; 
    
    [Tooltip("Drag the Player's Image here if it's a Canvas UI object")]
    public Image uiImage; 
    
    [Header("Animation Frames")]
    [Tooltip("Drag your new player body fishing frames here!")]
    public Sprite[] fishingFrames;
    public float framesPerSecond = 12f;

    [Header("Idle / Default Sprite")]
    [Tooltip("Drag the default idle sprite of the player body here. After the fishing animation, it will go back to this.")]
    public Sprite idleSprite;

    private Sprite originalSprite; // Automatically saved at start as a fallback

    [Header("3D Head Tracking")]
    [Tooltip("Drag your 3D Head object here so the script can move it")]
    public Transform headTransform;
    
    [Tooltip("Type the X and Y positions of where the neck is for each frame. Make sure this list has the exact same number of items as your fishingFrames list!")]
    public Vector3[] neckPositions;

    [ContextMenu("▶ TEST FISHING ANIMATION")]
    public void TestAnimation()
    {
        if (Application.isPlaying)
        {
            StartCoroutine(PlayAnimationOnce(null));
        }
        else
        {
            Debug.LogWarning("⚠️ You must press the actual PLAY button at the top of Unity first! Then you can click the 3 dots to test the animation.");
        }
    }

    void Start()
    {
        // Remember whatever sprite is showing right now as the "idle" fallback
        if (spriteRenderer != null) originalSprite = spriteRenderer.sprite;
        if (uiImage != null) originalSprite = uiImage.sprite;
    }

    // Added a callback parameter so the Sequence Manager knows when it finishes!
    public IEnumerator PlayAnimationOnce(System.Action onAnimationFinished)
    {
        Debug.Log($"[FishingPlayerAnimator] PlayAnimationOnce called on: {gameObject.name} | frames: {(fishingFrames != null ? fishingFrames.Length : 0)} | uiImage: {(uiImage != null ? uiImage.name : "NULL")} | spriteRenderer: {(spriteRenderer != null ? spriteRenderer.name : "NULL")}");

        if (fishingFrames == null || fishingFrames.Length == 0) 
        {
            Debug.LogError($"[FishingPlayerAnimator] No frames on {gameObject.name}!");
            yield break;
        }

        float timePerFrame = 1f / framesPerSecond;

        for (int i = 0; i < fishingFrames.Length; i++)
        {
            if (spriteRenderer != null) spriteRenderer.sprite = fishingFrames[i];
            if (uiImage != null) uiImage.sprite = fishingFrames[i];
            
            if (headTransform != null && neckPositions != null && i < neckPositions.Length)
            {
                headTransform.localPosition = neckPositions[i];
            }
            
            yield return new WaitForSeconds(timePerFrame);
        }
        
        // Tell the manager the animation is completely done!
        if (onAnimationFinished != null)
        {
            onAnimationFinished.Invoke();
        }
    }

    // Plays the frames BACKWARDS (reverse of the casting animation, like pulling the rod back)
    public IEnumerator PlayAnimationReverse(System.Action onAnimationFinished)
    {
        if (fishingFrames == null || fishingFrames.Length == 0)
        {
            if (onAnimationFinished != null) onAnimationFinished.Invoke();
            yield break;
        }

        float timePerFrame = 1f / framesPerSecond;

        // Loop backwards from the last frame to the first frame
        for (int i = fishingFrames.Length - 1; i >= 0; i--)
        {
            if (spriteRenderer != null) spriteRenderer.sprite = fishingFrames[i];
            if (uiImage != null) uiImage.sprite = fishingFrames[i];

            if (headTransform != null && neckPositions != null && i < neckPositions.Length)
            {
                headTransform.localPosition = neckPositions[i];
            }

            yield return new WaitForSeconds(timePerFrame);
        }

        // After the reverse animation, restore the idle sprite
        Sprite restoreSprite = idleSprite != null ? idleSprite : originalSprite;
        if (restoreSprite != null)
        {
            if (spriteRenderer != null) spriteRenderer.sprite = restoreSprite;
            if (uiImage != null) uiImage.sprite = restoreSprite;
        }

        if (onAnimationFinished != null)
        {
            onAnimationFinished.Invoke();
        }
    }
}
