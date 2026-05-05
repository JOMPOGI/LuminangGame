using UnityEngine;

public class OutfitItem : MonoBehaviour
{
    public enum Slot { Hair, Top, Bottom, Shoes, Accessories }

    public Slot slot;
    public Sprite icon;

    [Header("Body parts this item hides")]
    public bool hideTorso;
    public bool hideLegs;
    public bool hideArms;
    public bool hideHead;
    public bool hideFeet;
    public bool hideHands;
    public bool hideHips;
}