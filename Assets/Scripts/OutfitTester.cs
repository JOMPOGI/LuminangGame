using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class OutfitTester : MonoBehaviour
{
    public OutfitManager manager;
    public OutfitItem testItem;

    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current == null) return;

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            manager.Equip(testItem);
        }
        if (Keyboard.current.yKey.wasPressedThisFrame)
        {
            manager.Unequip(testItem.slot);
        }
#else
        if (Input.GetKeyDown(KeyCode.T))
        {
            manager.Equip(testItem);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            manager.Unequip(testItem.slot);
        }
#endif
    }
}