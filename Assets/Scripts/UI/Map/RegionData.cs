using UnityEngine;

[System.Serializable]
public class RegionData
{
    public string regionName;
    public string language;
    [TextArea(3, 10)]
    public string description;
    public Sprite thumbnail;
    public Sprite glowSprite;
    public Sprite crystalAnchorSprite;
    
    [Header("Zoom Settings")]
    public Vector3 zoomPosition;
    public float zoomOrthographicSize = 3f;
    public Vector2 zoomOffsetOverride; // For manual "nudging" per island!

    [Header("Progress")]
    [Range(0, 1)] 
    public float completionProgress = 0.0f; // 0.0 to 1.0 (0% to 100%)
}
