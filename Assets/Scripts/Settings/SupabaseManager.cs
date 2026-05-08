using UnityEngine;
using Supabase;
using System.Threading.Tasks;
using System;

public class SupabaseManager : MonoBehaviour
{
    public static SupabaseManager Instance { get; private set; }

    [Header("Supabase Credentials")]
    public string supabaseUrl;
    public string supabaseKey;

    public Client client;

    // Event to notify when Google login/callback is finished
    public event Action<bool> OnGoogleLoginComplete;

    private void LoadCredentials()
    {
        TextAsset configAsset = Resources.Load<TextAsset>("SupabaseConfig");
        if (configAsset != null)
        {
            string[] lines = configAsset.text.Split('\n');
            if (lines.Length >= 2)
            {
                supabaseUrl = lines[0].Trim();
                supabaseKey = lines[1].Trim();
            }
        }
        else
        {
            Debug.LogError("[Supabase] Missing SupabaseConfig.txt in Resources!");
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 1. Setup Infrastructure
            UnityMainThreadDispatcher.CheckInstance();
            
            // Add the Redirect Listener component
            if (GetComponent<UnityRedirectListener>() == null)
            {
                gameObject.AddComponent<UnityRedirectListener>();
            }
            if (GetComponent<UserProfileManager>() == null)
            {
                gameObject.AddComponent<UserProfileManager>();
            }
            if (GetComponent<SceneFader>() == null)
            {
                gameObject.AddComponent<SceneFader>();
            }

            LoadCredentials();
            InitializeSupabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSupabase()
    {
        // 2. Configure Unity-specific options
        var options = new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true,
            SessionHandler = new UnitySessionHandler()
        };

        client = new Client(supabaseUrl, supabaseKey, options);
        Debug.Log("<color=green>[Supabase] Client Initialized with Unity Support!</color>");
    }

    /// <summary>
    /// Processes the URL returned by the browser (from Editor or Mobile).
    /// </summary>
    public async void ProcessResultUrl(string url)
    {
        try
        {
            Debug.Log("[Supabase] Processing callback URL...");
            Debug.Log($"[Supabase] URL contains 'access_token': {url.Contains("access_token")}");
            Debug.Log($"[Supabase] URL contains 'code': {url.Contains("code=")}");
            Debug.Log($"[Supabase] URL contains 'error': {url.Contains("error")}");

            // Standard library method to convert URL -> Session
            var session = await client.Auth.GetSessionFromUrl(new Uri(url), true);
            
            Debug.Log($"[Supabase] GetSessionFromUrl returned. Session is null: {session == null}");
            if (session != null)
            {
                Debug.Log($"[Supabase] Session.User is null: {session.User == null}");
                if (session.User != null)
                    Debug.Log($"[Supabase] User ID: {session.User.Id}, Email: {session.User.Email}");
            }

            if (session != null && session.User != null)
            {
                Debug.Log("<color=green>[Supabase] Session caught successfully!</color>");
                OnGoogleLoginComplete?.Invoke(true);
            }
            else
            {
                Debug.LogWarning("[Supabase] Session or User was null after GetSessionFromUrl.");
                OnGoogleLoginComplete?.Invoke(false);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Supabase] Error processing callback: {ex.Message}");
            Debug.LogError($"[Supabase] Stack trace: {ex.StackTrace}");
            OnGoogleLoginComplete?.Invoke(false);
        }
    }
}

// =====================================================
// CENTRAL DATABASE MODELS
// =====================================================
[Postgrest.Attributes.Table("profiles")]
public class ProfileModel : Postgrest.Models.BaseModel
{
    [Postgrest.Attributes.PrimaryKey("id", false)]
    public string Id { get; set; }

    [Postgrest.Attributes.Column("email")]
    public string Email { get; set; }

    [Postgrest.Attributes.Column("username")]
    public string Username { get; set; }

    [Postgrest.Attributes.Column("equipped_outfit")]
    public object EquippedOutfit { get; set; } 

    [Postgrest.Attributes.Column("avatar_url")]
    public string AvatarUrl { get; set; }

    [Postgrest.Attributes.Column("has_created_character")]
    public bool HasCreatedCharacter { get; set; }

    [Postgrest.Attributes.Column("has_completed_tutorial")]
    public bool HasCompletedTutorial { get; set; }

    [Postgrest.Attributes.Column("has_seen_prologue")]
    public bool HasSeenPrologue { get; set; }

    [Postgrest.Attributes.Column("username_finalized_at")]
    public DateTime? UsernameFinalizedAt { get; set; }
}

[Postgrest.Attributes.Table("user_inventory")]
public class InventoryModel : Postgrest.Models.BaseModel
{
    // Remove ID property or use the correct type if we aren't providing it
    [Postgrest.Attributes.Column("user_id")]
    public string UserId { get; set; }

    [Postgrest.Attributes.Column("item_name")]
    public string ItemName { get; set; }

    [Postgrest.Attributes.Column("slot")]
    public string Slot { get; set; }
}
