using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Supabase.Gotrue;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Postgrest.Attributes;
using Postgrest.Models;

public class SignupManager : MonoBehaviour
{
    [Header("UI Fields")]
    public TMP_InputField usernameField;
    public TMP_InputField emailField;
    public TMP_InputField passwordField;

    [Header("Settings")]
    public string loginSceneName = "LoginScene";
    public string mainMenuSceneName = "MainMenuScene";

    [Header("Status")]
    public bool isBusy = false;

    private void Start()
    {
        Debug.Log("[Signup] Manager started. Subscribing to Google Login event...");
        if (SupabaseManager.Instance != null)
        {
            SupabaseManager.Instance.OnGoogleLoginComplete += HandleGoogleSignupComplete;
            Debug.Log("[Signup] Successfully subscribed to Supabase event.");
        }
        else
        {
            Debug.LogError("[Signup] SupabaseManager Instance was null at Start! Searching in scene...");
            var manager = FindFirstObjectByType<SupabaseManager>();
            if (manager != null)
            {
                manager.OnGoogleLoginComplete += HandleGoogleSignupComplete;
                Debug.Log("[Signup] Found and subscribed to SupabaseManager manually.");
            }
        }
    }

    private void OnDestroy()
    {
        if (SupabaseManager.Instance != null)
        {
            SupabaseManager.Instance.OnGoogleLoginComplete -= HandleGoogleSignupComplete;
        }
    }

    public async void OnSignupButtonClicked()
    {
        if (isBusy) return;

        string username = usernameField.text.Trim().ToLower();
        string email = emailField.text.Trim();
        string password = passwordField.text;

        if (!IsValidUsername(username))
        {
            GenericModal.Instance.ShowAlert("Username must be 3-16 characters and contain only lowercase letters and numbers.", "Okay");
            return;
        }
        if (!IsValidEmail(email))
        {
            GenericModal.Instance.ShowAlert("Please enter a valid email address.", "Okay");
            return;
        }
        if (!IsValidPassword(password))
        {
            GenericModal.Instance.ShowAlert("Password must be at least 8 characters, include an uppercase letter, a lowercase letter, and a number.", "Okay");
            return;
        }

        isBusy = true;
        LoadingOverlay.Instance?.Show();

        try
        {
            // 1. Check if Username is taken
            bool userExists = await CheckUsernameExists(username);
            if (userExists)
            {
                GenericModal.Instance.ShowAlert($"The username '{username}' is already taken.", "Okay");
                LoadingOverlay.Instance?.Hide();
                isBusy = false;
                return;
            }

            // 2. Check if Email is already in use (by Google or Manual)
            bool emailExists = await CheckEmailExists(email);
            if (emailExists)
            {
                GenericModal.Instance.ShowAlert("This email is already in use! If you used 'Continue with Google', please log in with that instead.", "Okay");
                LoadingOverlay.Instance?.Hide();
                isBusy = false;
                return;
            }

            var signupOptions = new SignUpOptions
            {
                Data = new Dictionary<string, object> { { "username", username } }
            };

            var response = await SupabaseManager.Instance.client.Auth.SignUp(email, password, signupOptions);
            
            if (response != null && response.User != null)
            {
                LoadingOverlay.Instance?.Hide();
                GenericModal.Instance.ShowAlert(
                    "Account created! Please verify your email before logging in.", 
                    "Okay", 
                    () => SceneManager.LoadScene(loginSceneName)
                );
            }
        }
        catch (System.Exception ex)
        {
            LoadingOverlay.Instance?.Hide();
            Debug.LogError($"[Signup] Error: {ex.Message}");
            
            string friendlyMessage = "Could not create account.";
            if (ex.Message.Contains("already been registered")) 
                friendlyMessage = "This email is already in use (possibly by a Google login)!";
            else if (ex.Message.Contains("Database error"))
                friendlyMessage = "Database error. Please try a different username.";

            GenericModal.Instance.ShowAlert(friendlyMessage, "Okay");
        }

        isBusy = false;
    }

    private bool IsValidUsername(string user)
    {
        if (user.Length < 3 || user.Length > 16) return false;
        return Regex.IsMatch(user, @"^[a-z0-9]+$");
    }

    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    private bool IsValidPassword(string pass)
    {
        return pass.Length >= 8 && Regex.IsMatch(pass, @"[A-Z]") && Regex.IsMatch(pass, @"[a-z]") && Regex.IsMatch(pass, @"[0-9]");
    }

    private async Task<bool> CheckEmailExists(string email)
    {
        try
        {
            var parameters = new Dictionary<string, object> { { "target_email", email.ToLower() } };
            var rpcResponse = await SupabaseManager.Instance.client.Rpc("check_email_exists", parameters);
            
            // The RPC returns a boolean directly
            if (rpcResponse != null && rpcResponse.Content != null)
            {
                return rpcResponse.Content.ToLower() == "true";
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Signup] RPC Error checking email: {ex.Message}");
        }
        return false;
    }

    private async Task<bool> CheckUsernameExists(string user)
    {
        var result = await SupabaseManager.Instance.client
            .From<ProfileModel>()
            .Filter("username", Postgrest.Constants.Operator.Equals, user)
            .Get();

        return result.Models.Count > 0;
    }

    public void OnContinueWithGoogleButtonClicked()
    {
        if (isBusy) return;

        isBusy = true;
        LoadingOverlay.Instance?.Show();

        try
        {
#if UNITY_EDITOR
            if (UnityRedirectListener.Instance != null)
            {
                UnityRedirectListener.Instance.StartEditorListener();
            }
#endif

            var redirectTo = "luminang://callback"; 
#if UNITY_EDITOR
            redirectTo = "http://localhost:54321/"; 
#endif
            string authUrl = $"{SupabaseManager.Instance.supabaseUrl}/auth/v1/authorize?provider=google&redirect_to={redirectTo}";
            
            Debug.Log($"[Signup] Opening Google signup in browser...");
            Application.OpenURL(authUrl);
        }
        catch (System.Exception ex)
        {
            LoadingOverlay.Instance?.Hide();
            Debug.LogError($"[Signup] Google Error: {ex.Message}");
            GenericModal.Instance.ShowAlert("Google signup failed.", "Okay");
            isBusy = false;
        }
    }

    private void HandleGoogleSignupComplete(bool success)
    {
        Debug.Log($"[Signup] HandleGoogleSignupComplete called. Success: {success}");

        if (!success)
        {
            LoadingOverlay.Instance?.Hide();
            GenericModal.Instance.ShowAlert("Google signup failed or was cancelled.", "Okay");
            isBusy = false;
            return;
        }

        Debug.Log("<color=green>[Signup] Google signup successful! Transitioning to Main Menu...</color>");
        LoadingOverlay.Instance?.Hide();
        
        // For signup, we trust the DB trigger to handle his profile.
        // We load the Main Menu directly.
        SceneManager.LoadScene(mainMenuSceneName);
        isBusy = false;
    }
}

