using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections.Generic;
using Postgrest.Attributes;
using Postgrest.Models;
using System.Linq;
using Supabase.Gotrue;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [Header("UI Fields")]
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public Button forgotPasswordButton;

    [Header("Reset Password Settings")]
    public string resetPasswordUrl = "https://www.luminang.com/auth/reset-password";

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

        if (forgotPasswordButton != null)
        {
            forgotPasswordButton.onClick.AddListener(OnForgotPasswordClicked);
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

            string foundEmail = "";

            // 1. Check if the user typed an email directly
            if (username.Contains("@"))
            {
                Debug.Log("[Login] User entered an email directly. Verifying if it exists...");
                
                // NEW: Verify if this email exists before trying to log in
                var emailParams = new Dictionary<string, object> { { "target_email", username.ToLower() } };
                var emailCheck = await SupabaseManager.Instance.client.Rpc("check_email_exists", emailParams);
                
                if (emailCheck == null || emailCheck.Content.ToLower() != "true")
                {
                    GenericModal.Instance.ShowAlert($"No account found for '{username}'. Please sign up first!", "Okay");
                    LoadingOverlay.Instance?.Hide();
                    isBusy = false;
                    return;
                }

                foundEmail = username;
            }
            else
            {
                // 2. Lookup email by username
                Debug.Log($"[Login] Looking up email for username: {username}");
                var parameters = new Dictionary<string, object> { { "target_username", username } };
                var rpcResponse = await SupabaseManager.Instance.client.Rpc("get_email_from_username", parameters);

                if (rpcResponse == null || string.IsNullOrEmpty(rpcResponse.Content) || rpcResponse.Content == "null")
                {
                    GenericModal.Instance.ShowAlert($"The username '{username}' does not exist.", "Okay");
                    LoadingOverlay.Instance?.Hide();
                    isBusy = false;
                    return;
                }
                foundEmail = rpcResponse.Content.Trim('\"');
            }

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

    private bool _waitingForGoogleLogin = false;

    private Coroutine _googleLoginTimeoutCoroutine;

    public void OnContinueWithGoogleButtonClicked()
    {
        if (isBusy) return;

        isBusy = true;
        _waitingForGoogleLogin = true;
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

            // Start a hard 60-second timeout just in case focus detection fails
            if (_googleLoginTimeoutCoroutine != null) StopCoroutine(_googleLoginTimeoutCoroutine);
            _googleLoginTimeoutCoroutine = StartCoroutine(GoogleLoginTimeout());
        }
        catch (System.Exception ex)
        {
            LoadingOverlay.Instance?.Hide();
            Debug.LogError($"[Login] Google Error: {ex.Message}");
            GenericModal.Instance.ShowAlert("Google login failed.", "Okay");
            isBusy = false;
            _waitingForGoogleLogin = false;
        }
    }

    private System.Collections.IEnumerator GoogleLoginTimeout()
    {
        yield return new WaitForSeconds(15f);
        if (_waitingForGoogleLogin)
        {
            Debug.Log("[Login] Google login timed out after 15 seconds.");
            CancelGoogleLogin();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && _waitingForGoogleLogin)
        {
            // Give the deep link a short moment to arrive
            StartCoroutine(CheckGoogleLoginCancelled());
        }
    }

    private System.Collections.IEnumerator CheckGoogleLoginCancelled()
    {
        yield return new WaitForSeconds(2f);
        if (_waitingForGoogleLogin)
        {
            Debug.Log("[Login] User returned but no deep link received. Cancelling loading.");
            CancelGoogleLogin();
        }
    }

    private void CancelGoogleLogin()
    {
        _waitingForGoogleLogin = false;
        if (isBusy)
        {
            isBusy = false;
            LoadingOverlay.Instance?.Hide();
            GenericModal.Instance.ShowAlert("Login cancelled or timed out. Please try again.", "Okay");
        }
    }

    private async void HandleGoogleLoginComplete(bool success)
    {
        _waitingForGoogleLogin = false;
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

    public void OnForgotPasswordClicked()
    {
        if (GenericModal.Instance == null) return;

        string currentInput = usernameField.text.Trim();

        // If they already typed an email in the box, just ask for confirmation
        if (!string.IsNullOrEmpty(currentInput) && currentInput.Contains("@"))
        {
            GenericModal.Instance.ShowConfirm(
                $"Send a password reset link to {currentInput}?",
                "Send Link",
                () => _ = SendPasswordReset(currentInput),
                "Cancel"
            );
        }
        else
        {
            // Otherwise, show the input modal so they can type it
            GenericModal.Instance.ShowInput(
                "Enter your email to receive a password reset link:",
                "Send Link",
                (email) => _ = SendPasswordReset(email),
                "Cancel"
            );
        }
    }

    private async Task SendPasswordReset(string email)
    {
        if (string.IsNullOrEmpty(email) || !email.Contains("@"))
        {
            GenericModal.Instance.ShowAlert("Please enter a valid email address first!");
            return;
        }

        LoadingOverlay.Instance?.Show();
        try
        {
            // Fix for the specific Supabase version syntax
            var options = new ResetPasswordForEmailOptions(email) { RedirectTo = resetPasswordUrl };
            await SupabaseManager.Instance.client.Auth.ResetPasswordForEmail(options);

            GenericModal.Instance.ShowAlert($"Success! Please check your email '{email}' for the reset link.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[Login] Reset Error: " + ex.Message);
            string friendlyMessage = TranslateResetError(ex.Message);
            GenericModal.Instance.ShowAlert(friendlyMessage);
        }
        finally
        {
            LoadingOverlay.Instance?.Hide();
        }
    }

    private string TranslateResetError(string technicalError)
    {
        string error = technicalError.ToLower();

        if (error.Contains("validation_failed") || error.Contains("requires an email"))
            return "Please enter a valid email address.";

        if (error.Contains("rate limit") || error.Contains("429"))
            return "Too many requests. Please wait a few minutes before trying again.";

        if (error.Contains("network") || error.Contains("timeout"))
            return "Connection error. Please check your internet.";

        return "Could not send reset link. Please double-check your email and try again.";
    }
}

