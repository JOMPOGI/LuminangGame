using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using Supabase.Storage;
using System.IO;

public class AvatarManager : MonoBehaviour
{
    public static AvatarManager Instance { get; private set; }

    [Header("Settings")]
    public string bucketName = "avatars";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public async Task<string> CaptureAndUpload(string userId, RenderTexture source)
    {
        if (source == null)
        {
            Debug.LogError("[AvatarManager] FAILED: The RenderTexture (source) is null! Check your PortraitBooth settings.");
            return null;
        }

        Debug.Log($"[AvatarManager] Capturing portrait for {userId} (Texture Size: {source.width}x{source.height})");

        try
        {
            // 1. Convert RenderTexture to Texture2D
            Texture2D tex = new Texture2D(source.width, source.height, TextureFormat.RGB24, false);
            RenderTexture.active = source;
            tex.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;

            byte[] bytes = tex.EncodeToPNG();
            Destroy(tex); 

            // 2. Upload to Supabase Storage
            string fileName = $"{userId}.png";
            var storage = SupabaseManager.Instance.client.Storage.From(bucketName);
            
            Debug.Log($"[AvatarManager] Attempting upload to bucket '{bucketName}' as '{fileName}'...");
            await storage.Upload(bytes, fileName, new Supabase.Storage.FileOptions { Upsert = true });
            
            // 3. Get the Public URL
            string publicUrl = storage.GetPublicUrl(fileName);
            Debug.Log($"[AvatarManager] UPLOAD SUCCESS! URL: {publicUrl}");

            // 4. Update the Profile table
            if (UserProfileManager.Instance != null && UserProfileManager.Instance.CurrentProfile != null)
            {
                var profile = UserProfileManager.Instance.CurrentProfile;
                profile.AvatarUrl = publicUrl;
                await UserProfileManager.Instance.UpdateProfile(profile);
                Debug.Log("[AvatarManager] Database updated with new Avatar URL.");
            }
            else
            {
                Debug.LogWarning("[AvatarManager] UserProfileManager.CurrentProfile is null. URL saved to storage but not to database profile.");
            }

            return publicUrl;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[AvatarManager] CRITICAL ERROR: {ex.Message}");
            if (ex.Message.Contains("403") || ex.Message.Contains("Policy"))
            {
                Debug.LogError("[AvatarManager] TIP: This looks like a PERMISSIONS issue. Go to Supabase Storage > Policies and allow Uploads!");
            }
            return null;
        }
    }
}
