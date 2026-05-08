using UnityEngine;

public class WebsiteLinker : MonoBehaviour
{
    private const string WebsiteUrl = "https://www.luminang.com/";

    /// <summary>
    /// Opens the Luminang website in the system's default browser.
    /// Works in Unity Editor, Android, and iOS.
    /// </summary>
    public void OpenLuminangWebsite()
    {
        Debug.Log($"[UI] Opening website: {WebsiteUrl}");
        Application.OpenURL(WebsiteUrl);
    }
}
