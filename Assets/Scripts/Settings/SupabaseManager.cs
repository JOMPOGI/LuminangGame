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
            // Standard library method to convert URL -> Session
            var session = await client.Auth.GetSessionFromUrl(new Uri(url), true);
            
            if (session != null && session.User != null)
            {
                Debug.Log("<color=green>[Supabase] Session caught successfully!</color>");
                OnGoogleLoginComplete?.Invoke(true);
            }
            else
            {
                OnGoogleLoginComplete?.Invoke(false);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Supabase] Error processing callback: {ex.Message}");
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
    [Postgrest.Attributes.Column("id")]
    public string Id { get; set; }

    [Postgrest.Attributes.Column("email")]
    public string Email { get; set; }

    [Postgrest.Attributes.Column("username")]
    public string Username { get; set; }
}
