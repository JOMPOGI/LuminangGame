using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MobileKeyboardOpener : MonoBehaviour, IPointerClickHandler
{
    private TMP_InputField inputField;
    private TouchScreenKeyboard keyboard;

    void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OpenKeyboard();
    }

    void OpenKeyboard()
    {
        if (inputField != null)
        {
            inputField.ActivateInputField();
            inputField.Select();

            keyboard = TouchScreenKeyboard.Open(
                inputField.text,
                TouchScreenKeyboardType.Default
            );
        }
    }
}