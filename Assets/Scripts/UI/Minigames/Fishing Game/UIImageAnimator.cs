using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIImageAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("Drag all your fish sprite frames in here.")]
    public Sprite[] frames;
    
    [Tooltip("How fast the animation plays.")]
    public float framesPerSecond = 12f;
    
    public bool loop = true;

    [Header("Movement Settings")]
    public bool canSwim = true;
    public float swimSpeed = 100f;
    
    [Tooltip("How far forward it swims before turning around and coming back to its start point.")]
    public float patrolDistance = 200f; 

    private Image imageComponent;
    private int currentFrame;
    private float timer;

    private Vector3 startPosition;
    private float initialDirection = 1f;
    private float currentDirection = 1f;

    void Start()
    {
        imageComponent = GetComponent<Image>();
        
        if (frames.Length > 0)
        {
            imageComponent.sprite = frames[0];
        }

        startPosition = transform.localPosition;
        
        // Uses the scale you manually set in the inspector to determine where it swims first
        initialDirection = Mathf.Sign(transform.localScale.x);
        currentDirection = initialDirection;
    }

    void Update()
    {
        AnimateSprite();
        
        if (canSwim)
        {
            MoveFish();
        }
    }

    void AnimateSprite()
    {
        if (frames == null || frames.Length == 0) 
            return;

        timer += Time.deltaTime;
        float timePerFrame = 1f / framesPerSecond;

        if (timer >= timePerFrame)
        {
            timer -= timePerFrame;
            currentFrame++;

            if (currentFrame >= frames.Length)
            {
                if (loop)
                {
                    currentFrame = 0;
                }
                else
                {
                    currentFrame = frames.Length - 1;
                }
            }

            imageComponent.sprite = frames[currentFrame];
        }
    }

    void MoveFish()
    {
        // Move the fish horizontally
        transform.localPosition += new Vector3(currentDirection * swimSpeed * Time.deltaTime, 0, 0);

        // Check how far it is from its original starting position
        float distanceFromStart = transform.localPosition.x - startPosition.x;

        if (initialDirection > 0)
        {
            // It started by going right. It should only patrol between 0 and patrolDistance.
            if (currentDirection > 0 && distanceFromStart >= patrolDistance)
            {
                // Reached the far right point, turn around
                transform.localPosition = new Vector3(startPosition.x + patrolDistance, transform.localPosition.y, transform.localPosition.z);
                TurnAround();
            }
            else if (currentDirection < 0 && distanceFromStart <= 0)
            {
                // Came back to the original start point, turn around to go right again
                transform.localPosition = new Vector3(startPosition.x, transform.localPosition.y, transform.localPosition.z);
                TurnAround();
            }
        }
        else
        {
            // It started by going left. It should only patrol between -patrolDistance and 0.
            if (currentDirection < 0 && distanceFromStart <= -patrolDistance)
            {
                // Reached the far left point, turn around
                transform.localPosition = new Vector3(startPosition.x - patrolDistance, transform.localPosition.y, transform.localPosition.z);
                TurnAround();
            }
            else if (currentDirection > 0 && distanceFromStart >= 0)
            {
                // Came back to the original start point, turn around to go left again
                transform.localPosition = new Vector3(startPosition.x, transform.localPosition.y, transform.localPosition.z);
                TurnAround();
            }
        }
    }

    void TurnAround()
    {
        // Reverse direction
        currentDirection *= -1f;
        
        // Flip the image visually to face the new direction
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * currentDirection; 
        transform.localScale = scale;
    }
}
