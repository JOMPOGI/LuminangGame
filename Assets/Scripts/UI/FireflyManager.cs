using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class FireflyManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Assign a soft glow/dot sprite here.")]
    public Sprite fireflySprite;
    public int spawnCount = 20;
    public Vector2 sizeRange = new Vector2(25, 55);
    
    [Header("Animation Settings")]
    public Vector2 speedRange = new Vector2(10, 40);
    public Vector2 twinkleSpeedRange = new Vector2(0.3f, 0.8f);
    public Vector2 lifetimeRange = new Vector2(5f, 15f); // Seconds before respawning
    public Color fireflyColor = new Color(1, 1, 1, 0.8f);

    private RectTransform container;
    private List<FireflyInstance> fireflies = new List<FireflyInstance>();

    private void Start()
    {
        container = GetComponent<RectTransform>();
        
        // AUTO-GENERATE DOT SPRITE: If no sprite is assigned, we build a soft one in code!
        if (fireflySprite == null)
        {
            fireflySprite = CreateSoftDotSprite();
        }

        SpawnFireflies();
    }

    private Sprite CreateSoftDotSprite()
    {
        int size = 32;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float center = size / 2f;
        float coreRadius = center * 0.4f; // Adjust this for a bigger/smaller "solid" center

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                float alpha = 0;

                if (dist < coreRadius)
                {
                    alpha = 1f; // Full solid core
                }
                else
                {
                    // Smooth falloff for the glow part
                    alpha = Mathf.Clamp01(1f - (dist - coreRadius) / (center - coreRadius));
                    alpha = Mathf.Pow(alpha, 2f); // Smooth out the glow
                }

                tex.SetPixel(x, y, new Color(1, 1, 1, alpha));
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    private void SpawnFireflies()
    {
        if (container == null) container = GetComponent<RectTransform>();

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject go = new GameObject("Firefly_Sparkle", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            go.transform.SetParent(container, false);

            Image img = go.GetComponent<Image>();
            img.sprite = fireflySprite;
            img.color = fireflyColor;
            img.raycastTarget = false; 

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition = GetRandomLocalPosition();

            float size = Random.Range(sizeRange.x, sizeRange.y);
            rect.sizeDelta = new Vector2(size, size);

            fireflies.Add(new FireflyInstance
            {
                rect = rect,
                cg = go.GetComponent<CanvasGroup>(),
                speed = Random.Range(speedRange.x, speedRange.y),
                twinkleSpeed = Random.Range(twinkleSpeedRange.x, twinkleSpeedRange.y),
                targetPos = GetRandomLocalPosition(),
                timeOffset = Random.Range(0f, 100f),
                lifetime = Random.Range(lifetimeRange.x, lifetimeRange.y),
                age = 0f
            });
        }
    }

    private void Update()
    {
        if (container == null) return;

        float dt = Time.deltaTime;
        float time = Time.time;

        foreach (var f in fireflies)
        {
            // 0. LIFETIME & RESPAWN
            f.age += dt;
            if (f.age >= f.lifetime)
            {
                // Reset to new location and reset age
                f.rect.anchoredPosition = GetRandomLocalPosition();
                f.targetPos = GetRandomLocalPosition();
                f.age = 0f;
                f.lifetime = Random.Range(lifetimeRange.x, lifetimeRange.y);
            }

            // 1. WANDERING MOVEMENT
            f.rect.anchoredPosition = Vector2.MoveTowards(f.rect.anchoredPosition, f.targetPos, f.speed * dt);
            if (Vector2.Distance(f.rect.anchoredPosition, f.targetPos) < 5f)
            {
                f.targetPos = GetRandomLocalPosition();
            }

            // 2. "BEATING" GLOW + OVERALL FADE (Fade out near end of life)
            float t = (time * f.twinkleSpeed + f.timeOffset) % 1f;
            float pulse = Mathf.Exp(-5f * t) * Mathf.Sin(10f * t); 
            pulse = Mathf.Clamp01(pulse * 2f); 

            // Calculate overall master alpha based on life (fade in at start, fade out at end)
            float lifePercent = f.age / f.lifetime;
            float lifeFade = Mathf.Sin(lifePercent * Mathf.PI); // Smooth arc from 0 to 1 back to 0

            f.cg.alpha = Mathf.Lerp(0.02f, 1f, pulse) * lifeFade;
            
            float scalePulse = Mathf.Lerp(0.7f, 1.3f, pulse);
            f.rect.localScale = new Vector3(scalePulse, scalePulse, 1f);
        }
    }

    private Vector2 GetRandomLocalPosition()
    {
        if (container == null) return Vector2.zero;
        float x = Random.Range(-container.rect.width / 2f, container.rect.width / 2f);
        float y = Random.Range(-container.rect.height / 2f, container.rect.height / 2f);
        return new Vector2(x, y);
    }

    private class FireflyInstance
    {
        public RectTransform rect;
        public CanvasGroup cg;
        public float speed;
        public float twinkleSpeed;
        public Vector2 targetPos;
        public float timeOffset;
        public float lifetime;
        public float age;
    }
}
