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

    [Header("Status")]
    public bool isBusy = false;

    public async void OnSignupButtonClicked()
    {
        if (isBusy) return;

        string username = usernameField.text.Trim();
        string email = emailField.text.Trim();
        string password = passwordField.text;

        // 1. LOCAL VALIDATION
        if (!IsValidUsername(username)) return;
        if (!IsValidEmail(email)) return;
        if (!IsValidPassword(password)) return;

        isBusy = true;
        LoadingOverlay.Instance?.Show();

        try
        {
            // 1. Check if username exists
            bool exists = await CheckUsernameExists(username);
            if (exists)
            {
                GenericModal.Instance.ShowAlert($"The username '{username}' is already taken. Please choose another one.", "Okay");
                LoadingOverlay.Instance?.Hide();
                isBusy = false;
                return;
            }

            // 2. Validate email again (extra safety)
            if (!IsValidEmail(email))
            {
                GenericModal.Instance.ShowAlert("Please enter a valid email address.", "Okay");
                LoadingOverlay.Instance?.Hide();
                isBusy = false;
                return;
            }

            // 3. ATTEMPT SIGNUP
            Debug.Log($"[Signup] Attempting signup for: {email}");
            var signupOptions = new SignUpOptions
            {
                Data = new Dictionary<string, object> { { "username", username } }
            };

            var response = await SupabaseManager.Instance.client.Auth.SignUp(email, password, signupOptions);
            
            if (response != null && response.User != null)
            {
                Debug.Log("<color=green>[Signup] Success!</color>");
                LoadingOverlay.Instance?.Hide();
                GenericModal.Instance.ShowAlert(
                    "Account created! Please check your email for a confirmation link before logging in.", 
                    "Okay", 
                    () => SceneManager.LoadScene(loginSceneName)
                );
            }
        }
        catch (System.Exception ex)
        {
            LoadingOverlay.Instance?.Hide();
            // Log for dev
            Debug.LogError($"[Signup] Technical Error: {ex.Message}");
            
            // Friendly message for player
            string friendlyMessage = "We couldn't create your account right now. Please try again later.";

            if (ex.Message.Contains("already registered"))
            {
                friendlyMessage = "This email is already in use. Try logging in instead!";
            }
            else if (ex.Message.Contains("network") || ex.Message.Contains("connection"))
            {
                friendlyMessage = "Connection error. Please check your internet and try again.";
            }

            GenericModal.Instance.ShowAlert(friendlyMessage, "Okay");
        }

        isBusy = false;
    }

    // --- Validation Logic ---

    private bool IsValidUsername(string user)
    {
        if (user.Length < 3 || user.Length > 15)
        {
            GenericModal.Instance.ShowAlert("Username must be between 3 and 15 characters.", "Okay");
            return false;
        }
        // Allow alphanumeric + special characters
        if (!Regex.IsMatch(user, @"^[a-zA-Z0-9!@#$%^&*()_+=-]+$"))
        {
            GenericModal.Instance.ShowAlert("Username contains invalid characters.", "Okay");
            return false;
        }
        return true;
    }

    private bool IsValidEmail(string email)
    {
        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            GenericModal.Instance.ShowAlert("Please enter a valid email address.", "Okay");
            return false;
        }
        return true;
    }

    private bool IsValidPassword(string pass)
    {
        // 8 chars, 1 upper, 1 lower, 1 number
        if (pass.Length < 8 || 
            !Regex.IsMatch(pass, @"[A-Z]") || 
            !Regex.IsMatch(pass, @"[a-z]") || 
            !Regex.IsMatch(pass, @"[0-9]"))
        {
            GenericModal.Instance.ShowAlert("Password must be at least 8 characters, with 1 uppercase, 1 lowercase, and 1 number.", "Okay");
            return false;
        }
        return true;
    }

    private async Task<bool> CheckUsernameExists(string user)
    {
        // Query the 'profiles' table we just created in Supabase
        var result = await SupabaseManager.Instance.client
            .From<ProfileModel>()
            .Filter("username", Postgrest.Constants.Operator.Equals, user)
            .Get();

        return result.Models.Count > 0;
    }
}

// Simple model for the profiles table
[Table("profiles")]
public class ProfileModel : BaseModel
{
    [Column("username")]
    public string Username { get; set; }
}
