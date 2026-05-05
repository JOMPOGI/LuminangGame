using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class CreateCharacterManager : MonoBehaviour
{
    [Header("Username")]
    public TMP_InputField usernameField;

    [Header("Save Button")]
    public Button saveButton;

    [Header("Modal")]
    public GenericModal modal;

    [Header("Flow")]
    public SceneLoader sceneLoader;
    public string nextSceneName = "PrologueScene";

    void Start()
    {
        saveButton.onClick.AddListener(OnSaveClicked);
        
        // Force InputField to follow the rules (3-16 chars, no spaces, no special chars)
        if (usernameField != null)
        {
            usernameField.characterLimit = 16;
            usernameField.contentType = TMP_InputField.ContentType.Alphanumeric; // Blocks symbols/spaces
            usernameField.onValueChanged.AddListener(val => usernameField.text = val.ToLower()); // Force lower
        }

        // Try to pre-fill the username from current profile
        _ = PreFillUsername();
    }

    private async Task PreFillUsername()
    {
        try
        {
            var user = SupabaseManager.Instance.client.Auth.CurrentUser;
            if (user == null) return;

            var response = await SupabaseManager.Instance.client
                .From<ProfileModel>()
                .Where(x => x.Id == user.Id)
                .Single();

            if (response != null && !string.IsNullOrEmpty(response.Username))
            {
                usernameField.text = response.Username;
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("[CreateCharacter] No profile found yet: " + ex.Message);
        }
    }

    private async void OnSaveClicked()
    {
        string username = usernameField.text.Trim();

        // 1. Basic Local Validation
        if (string.IsNullOrEmpty(username) || username.Length < 3)
        {
            modal.ShowAlert("Username must be between 3 and 16 characters.");
            return;
        }

        saveButton.interactable = false; // Prevent double clicking

        // 2. Check if name is taken in Supabase
        try
        {
            bool isTaken = await CheckIfUsernameTaken(username);
            if (isTaken)
            {
                modal.ShowAlert("Sorry, that username is already taken. Try another one!");
                saveButton.interactable = true;
                return;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[CreateCharacter] Username check failed: " + ex.Message);
            modal.ShowAlert("Could not verify username. Please check your internet connection and try again.");
            saveButton.interactable = true;
            return;
        }

        // 3. Show the "Are you sure?" confirmation
        modal.ShowConfirm(
            "Are you sure you want to save your character?",
            "Yes",
            () => _ = OnConfirmSave(username),
            "No",
            () => saveButton.interactable = true
        );
    }

    private async Task<bool> CheckIfUsernameTaken(string username)
    {
        var currentUser = SupabaseManager.Instance.client.Auth.CurrentUser;
        
        var response = await SupabaseManager.Instance.client
            .From<ProfileModel>()
            .Filter("username", Postgrest.Constants.Operator.Equals, username)
            .Get();

        // If we found any rows with this username
        if (response.Models.Count > 0)
        {
            // If the ID is different from ours, it's definitely taken
            foreach (var model in response.Models)
            {
                if (model.Id != currentUser.Id) 
                {
                    Debug.LogWarning($"[CreateCharacter] Username '{username}' is taken by user {model.Id}");
                    return true;
                }
            }
        }
        return false;
    }

    private async Task OnConfirmSave(string username)
    {
        try 
        {
            var client = SupabaseManager.Instance.client;
            var user = client.Auth.CurrentUser;
            var outfitManager = FindFirstObjectByType<OutfitManager>();

            if (outfitManager == null) {
                modal.ShowAlert("System Error: OutfitManager not found!");
                return;
            }

            // 1. Collect Outfit Data
            var equipped = outfitManager.GetEquippedNames();

            // 2. Update Profile
            var profileUpdate = new ProfileModel
            {
                Id = user.Id,
                Email = user.Email, 
                Username = username,
                EquippedOutfit = equipped, 
                HasCreatedCharacter = true,
                HasSeenPrologue = false, 
                HasCompletedTutorial = false
            };
            
            Debug.Log($"[CreateCharacter] Attempting to save profile for {user.Id}...");
            
            if (UserProfileManager.Instance != null)
            {
                await UserProfileManager.Instance.UpdateProfile(profileUpdate);
                Debug.Log("[CreateCharacter] UserProfileManager reported success.");
            }
            else
            {
                var response = await client.From<ProfileModel>().Upsert(profileUpdate);
                if (response.ResponseMessage != null && !response.ResponseMessage.IsSuccessStatusCode)
                {
                    throw new System.Exception($"Profile Update Failed: {response.ResponseMessage.ReasonPhrase}");
                }
                Debug.Log("[CreateCharacter] Direct Upsert reported success.");
            }

            // 3. Update Inventory (Welcome Pack)
            // Using Upsert so we don't create duplicates but also don't touch existing rewards!
            var inventoryItems = new List<InventoryModel>();
            foreach (var kv in equipped.ToDictionary())
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    inventoryItems.Add(new InventoryModel 
                    { 
                        UserId = user.Id, 
                        ItemName = kv.Value, 
                        Slot = kv.Key 
                    });
                }
            }
            
            if (inventoryItems.Count > 0)
                await client.From<InventoryModel>().Upsert(inventoryItems);

            // 4. Success!
            modal.ShowAlert(
                $"Character Created!\nWelcome, {username}!",
                "Okay",
                () => OnWelcomeDismissed()
            );
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[CreateCharacter] Save error: " + ex.Message);
            modal.ShowAlert(TranslateError(ex.Message));
            saveButton.interactable = true;
        }
    }

    private void OnWelcomeDismissed()
    {
        if (sceneLoader != null)
        {
            sceneLoader.LoadScene(nextSceneName);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
    }

    private string TranslateError(string technicalError)
    {
        string error = technicalError.ToLower();

        if (error.Contains("403") || error.Contains("forbidden"))
            return "Permission denied. Please try logging out and back in to refresh your session.";
        
        if (error.Contains("duplicate") || error.Contains("23505") || error.Contains("already exists"))
            return "Sorry, that username is already taken! Please try a different one.";

        if (error.Contains("network") || error.Contains("timeout") || error.Contains("connection"))
            return "Connection issue. Please check your internet and try again.";

        if (error.Contains("null value"))
            return "Missing information. Please make sure your username is filled out correctly.";

        return "Something went wrong while saving your character. Please try again!\n\n(Technical Error: " + technicalError + ")";
    }
}
