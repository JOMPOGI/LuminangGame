using UnityEngine;

public class OutfitItem : MonoBehaviour
{
    public enum Slot { Hair, Top, Bottom, Shoes, Accessories }

    public Slot slot;
    public Sprite icon;

    [Header("Display Info")]
    public string itemName;
    [TextArea(2, 4)]
    public string itemDescription;
    public int price = 100; // Default price

    [Header("Body parts this item hides")]
    public bool hideTorso;
    public bool hideLegs;
    public bool hideArms;
    public bool hideHead;
    public bool hideFeet;
    public bool hideHands;
    public bool hideHips;
}