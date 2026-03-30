using UnityEngine;
using Supabase;
using System.Threading.Tasks;

public class SupabaseManager : MonoBehaviour
{
    public static SupabaseManager Instance { get; private set; }

    [Header("Supabase Credentials")]
    public string supabaseUrl;
    public string supabaseKey;

    public Client client;

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
        var options = new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        };

        client = new Client(supabaseUrl, supabaseKey, options);
        Debug.Log("<color=green>[Supabase] Client Initialized!</color>");
    }
}
