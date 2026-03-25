using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField emailField;
    public TMP_InputField passwordField;

    [Header("UI Elements")]
    public GameObject errorText;

    [Header("Transition Settings")]
    public float transitionDelay = 0.4f;

    public void OnLoginButtonClicked()
    {
        string email = emailField.text.Trim();
        string password = passwordField.text;

        if (email == "luminang@gmail.com" && password == "Luminang2026!")
        {
            Debug.Log("Login successful. Loading MainMenuScene.");
            StartCoroutine(LoadMainMenuWithDelay());
        }
        else
        {
            Debug.Log("Invalid credentials.");
            if (errorText != null)
            {
                errorText.SetActive(true);
            }
        }
    }

    private System.Collections.IEnumerator LoadMainMenuWithDelay()
    {
        yield return new WaitForSeconds(transitionDelay);
        SceneManager.LoadScene("MainMenuScene");
    }
}
