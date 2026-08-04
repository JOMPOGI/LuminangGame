using UnityEngine;
using UnityEngine.UI;

// Attach this to any Button or Toggle GameObject.
// Drag an AudioSource and your click clip into the Inspector slots.
public class ButtonSFX : MonoBehaviour
{
    public AudioSource sfxSource;
    public AudioClip clickSFX;

    void Start()
    {
        // Check if it's a Button
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(PlayClick);
        }

        // Check if it's a Toggle instead
        Toggle tgl = GetComponent<Toggle>();
        if (tgl != null)
        {
            tgl.onValueChanged.AddListener((bool isOn) => PlayClick());
        }
    }

    void PlayClick()
    {
        if (sfxSource != null && clickSFX != null)
            sfxSource.PlayOneShot(clickSFX);
    }
}
