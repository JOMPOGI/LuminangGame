using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Postgrest.Attributes;
using Postgrest.Models;
using System.Linq;

public class LoginManager : MonoBehaviour
{
    [Header("UI Fields")]
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;

    [Header("Settings")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Status")]
    public bool isBusy = false;

    public async void OnLoginButtonClicked()
    {
        if (isBusy) return;

        string username = usernameField.text.Trim().ToLower();
        string password = passwordField.text;


        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            GenericModal.Instance.ShowAlert("Please enter both username and password.", "Okay");
            return;
        }

        // --- DEMO ACCOUNT BYPASS ---
        if (username == "luminang" && password == "Luminang2026!")
        {
            Debug.Log("<color=green>[Login] Demo account logged in successfully! (Database bypassed)</color>");
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }
        // ---------------------------

        isBusy = true;
        LoadingOverlay.Instance?.Show();

        try
        {
            // SAFETY CHECK: Is SupabaseManager ready?
            if (SupabaseManager.Instance == null || SupabaseManager.Instance.client == null)
            {
                GenericModal.Instance.ShowAlert("Supabase is not initialized. Please make sure the 'SupabaseManager' object is in your scene.", "Okay");
                LoadingOverlay.Instance?.Hide();
                isBusy = false;
                return;
            }

            // 1. LOOKUP EMAIL BY USERNAME
            Debug.Log($"[Login] Looking up email for user: {username}");
            
            var profileResult = await SupabaseManager.Instance.client
                .From<LoginProfileModel>()
                .Filter("username", Postgrest.Constants.Operator.Equals, username)
                .Get();

            if (profileResult == null)
            {
                Debug.LogError("[Login] profileResult is NULL");
                GenericModal.Instance.ShowAlert("No response from database.", "Okay");
                LoadingOverlay.Instance?.Hide();
                isBusy = false;
                return;
            }

            if (profileResult.Models == null)
            {
                Debug.LogError("[Login] profileResult.Models is NULL");
                GenericModal.Instance.ShowAlert("Database returned an empty response structure.", "Okay");
                LoadingOverlay.Instance?.Hide();
                isBusy = false;
                return;
            }

            Debug.Log($"[Login] Query returned {profileResult.Models.Count} results.");

            var profile = profileResult.Models.FirstOrDefault();

            if (profile == null)
            {
                Debug.LogWarning($"[Login] No profile found for username: {username}");
                GenericModal.Instance.ShowAlert($"The username '{username}' does not exist. Please check your spelling!", "Okay");
                LoadingOverlay.Instance?.Hide();
                isBusy = false;
                return;
            }

            if (string.IsNullOrEmpty(profile.Email))
            {
                Debug.LogWarning($"[Login] Profile found but Email is NULL for: {username}");
                GenericModal.Instance.ShowAlert("Your account exists, but your email is missing in the database. Please try signing up again.", "Okay");
                LoadingOverlay.Instance?.Hide();
                isBusy = false;
                return;
            }

            Debug.Log($"[Login] Successfully found email: {profile.Email}. Authenticating...");

            // 2. ATTEMPT SIGN IN WITH THE FOUND EMAIL
            var response = await SupabaseManager.Instance.client.Auth.SignIn(profile.Email, password);

            if (response != null && response.User != null)
            {
                Debug.Log("<color=green>[Login] User Authenticated successfully!</color>");
                LoadingOverlay.Instance?.Hide();
                SceneManager.LoadScene(mainMenuSceneName);
            }
        }
        catch (System.Exception ex)
        {
            LoadingOverlay.Instance?.Hide();
            // We keep the REAL error in the console for YOU (the dev) to see
            Debug.LogError($"[Login] Technical Error: {ex.Message}");
            
            // But we show the PLAYER a friendly, safe message
            string friendlyMessage = "Oops! Something went wrong. Please try again later.";

            if (ex.Message.Contains("Invalid login credentials") || ex.Message.Contains("not found"))
            {
                friendlyMessage = "Incorrect username or password. Please try again!";
            }
            else if (ex.Message.Contains("Email not confirmed"))
            {
                friendlyMessage = "Your email isn't confirmed yet! Please check your inbox for the confirmation link.";
            }
            else if (ex.Message.Contains("network") || ex.Message.Contains("connection"))
            {
                friendlyMessage = "Connection error. Please check your internet and try again.";
            }

            GenericModal.Instance.ShowAlert(friendlyMessage, "Okay");
        }

        isBusy = false;
    }
}

// Model for lookup (needs email)
[Table("profiles")]
public class LoginProfileModel : BaseModel
{
    [Column("username")]
    public string Username { get; set; }

    [Column("email")]
    public string Email { get; set; }
}
