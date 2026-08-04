using UnityEngine;
using UnityEngine.UI;

// Attach this to your Catch Button alongside the Button component.
// It watches if the button is disabled and grays out the image automatically.
[RequireComponent(typeof(Button))]
public class ButtonGrayout : MonoBehaviour
{
    [Tooltip("The color to use when the button is disabled. Default is a semi-transparent gray.")]
    public Color disabledColor = new Color(0.4f, 0.4f, 0.4f, 0.6f);

    private Button button;
    private Image buttonImage;
    private Color originalColor;
    private bool wasInteractable;

    void Awake()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        if (buttonImage != null)
            originalColor = buttonImage.color;

        wasInteractable = button.interactable;
        ApplyState(button.interactable);
    }

    void Update()
    {
        // Only update when the state actually changes to avoid doing work every frame
        if (button.interactable != wasInteractable)
        {
            wasInteractable = button.interactable;
            ApplyState(button.interactable);
        }
    }

    void ApplyState(bool interactable)
    {
        if (buttonImage == null) return;

        buttonImage.color = interactable ? originalColor : disabledColor;
    }
}
