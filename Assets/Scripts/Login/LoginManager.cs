using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections.Generic;
using Postgrest.Attributes;
using Postgrest.Models;
using System.Linq;
using Supabase.Gotrue;

public class LoginManager : MonoBehaviour
{
    [Header("UI Fields")]
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;

    [Header("Settings")]
    public string mainMenuSceneName = "MainMenuScene";

    [Header("Status")]
    public bool isBusy = false;

    private void Start()
    {
        Debug.Log("[Login] Manager started. Subscribing to Google Login event...");
        if (SupabaseManager.Instance != null)
        {
            SupabaseManager.Instance.OnGoogleLoginComplete += HandleGoogleLoginComplete;
            Debug.Log("[Login] Successfully subscribed to Supabase event.");
        }
        else
        {
            Debug.LogError("[Login] SupabaseManager Instance was null at Start! Searching in scene...");
            var manager = FindFirstObjectByType<SupabaseManager>();
            if (manager != null)
            {
                manager.OnGoogleLoginComplete += HandleGoogleLoginComplete;
                Debug.Log("[Login] Found and subscribed to SupabaseManager manually.");
            }
        }
    }

    private void OnDestroy()
    {
        if (SupabaseManager.Instance != null)
        {
            SupabaseManager.Instance.OnGoogleLoginComplete -= HandleGoogleLoginComplete;
        }
    }

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
            Debug.Log("<color=green>[Login] Demo account logged in successfully!</color>");
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }

        isBusy = true;
        LoadingOverlay.Instance?.Show();

        try
        {
            if (SupabaseManager.Instance == null || SupabaseManager.Instance.client == null)
            {
                GenericModal.Instance.ShowAlert("Supabase is not initialized.", "Okay");
                LoadingOverlay.Instance?.Hide();
                isBusy = false;
                return;
            }

            Debug.Log($"[Login] Looking up email for user: {username}");
            var parameters = new Dictionary<string, object> { { "target_username", username } };
            var rpcResponse = await SupabaseManager.Instance.client.Rpc("get_email_from_username", parameters);

            if (rpcResponse == null || string.IsNullOrEmpty(rpcResponse.Content) || rpcResponse.Content == "null")
            {
                GenericModal.Instance.ShowAlert($"The username '{username}' does not exist.", "Okay");
                LoadingOverlay.Instance?.Hide();
                isBusy = false;
                return;
            }

            string foundEmail = rpcResponse.Content.Trim('\"');
            var response = await SupabaseManager.Instance.client.Auth.SignIn(foundEmail, password);

            if (response != null && response.User != null)
            {
                Debug.Log("<color=green>[Login] Success! Fetching profile...</color>");
                
                // Fetch profile data before moving
                if (UserProfileManager.Instance != null)
                {
                    await UserProfileManager.Instance.FetchProfile();
                }

                LoadingOverlay.Instance?.Hide();
                SceneManager.LoadScene(mainMenuSceneName);
            }
        }
        catch (System.Exception ex)
        {
            LoadingOverlay.Instance?.Hide();
            Debug.LogError($"[Login] Technical Error: {ex.Message}");
            string friendlyMessage = "Oops! Something went wrong.";

            if (ex.Message.Contains("Invalid login credentials")) friendlyMessage = "Incorrect username or password.";
            else if (ex.Message.Contains("Email not confirmed")) friendlyMessage = "Please verify your email first!";
            
            GenericModal.Instance.ShowAlert(friendlyMessage, "Okay");
        }

        isBusy = false;
    }

    public void OnContinueWithGoogleButtonClicked()
    {
        if (isBusy) return;

        isBusy = true;
        LoadingOverlay.Instance?.Show();

        try
        {
            // 1. Tell the Listener to start (For Editor testing)
#if UNITY_EDITOR
            if (UnityRedirectListener.Instance != null)
            {
                UnityRedirectListener.Instance.StartEditorListener();
            }
#endif

            // 2. Build the manual URL (Most reliable)
            var redirectTo = "luminang://callback"; 
#if UNITY_EDITOR
            redirectTo = "http://localhost:54321/"; 
#endif
            string authUrl = $"{SupabaseManager.Instance.supabaseUrl}/auth/v1/authorize?provider=google&redirect_to={redirectTo}";
            
            Debug.Log($"[Login] Opening Google login in browser...");
            Application.OpenURL(authUrl);

            // Note: We don't hide the loading screen here. 
            // We wait for HandleGoogleLoginComplete to be called.
        }
        catch (System.Exception ex)
        {
            LoadingOverlay.Instance?.Hide();
            Debug.LogError($"[Login] Google Error: {ex.Message}");
            GenericModal.Instance.ShowAlert("Google login failed.", "Okay");
            isBusy = false;
        }
    }

    private async void HandleGoogleLoginComplete(bool success)
    {
        Debug.Log($"[Login] HandleGoogleLoginComplete called. Success: {success}");
        
        if (!success)
        {
            LoadingOverlay.Instance?.Hide();
            GenericModal.Instance.ShowAlert("Google login was cancelled or failed. Please try again.", "Okay");
            isBusy = false;
            return;
        }

        try
        {
            var session = SupabaseManager.Instance.client.Auth.CurrentSession;
            if (session != null && session.User != null)
            {
                Debug.Log($"[Login] Session verified. User ID: {session.User.Id}. Checking database for profile...");
                
                /* 
                // NOTE: We have removed the 'No Account Found' shield. 
                // Modern games allow 'Auto-Signup' via Google. 
                // The database trigger will automatically create the profile.
                */

                Debug.Log("<color=green>[Login] Google login successful! Fetching profile...</color>");
                
                // Fetch profile data before moving
                if (UserProfileManager.Instance != null)
                {
                    await UserProfileManager.Instance.FetchProfile();
                }

                LoadingOverlay.Instance?.Hide();
                SceneManager.LoadScene(mainMenuSceneName);
            }
            else
            {
                Debug.LogError("[Login] Login succeeded but session is null!");
                LoadingOverlay.Instance?.Hide();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Login] Critical Error in HandleComplete: {ex.Message}");
            LoadingOverlay.Instance?.Hide();
        }

        isBusy = false;
    }
}

