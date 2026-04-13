using System;
using UnityEngine;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using System.Threading.Tasks;

/// <summary>
/// PURPOSE: This script tells Supabase how to handle "Sessions" (staying logged in) 
/// and how to open the web browser on different platforms (PC/Mobile).
/// 
/// HOW IT WORKS: 
/// 1. It saves your login token to Unity's PlayerPrefs so you don't have to log in every time you open the game.
/// 2. It overrides the default browser logic to use Unity's Application.OpenURL.
/// </summary>
public class UnitySessionHandler : IGotrueSessionPersistence<Session>
{
    private const string SessionKey = "supabase_session";

    /// <summary>
    /// Saves the session to the device's local storage (PlayerPrefs).
    /// </summary>
    public void SaveSession(Session session)
    {
        if (session != null)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(session);
            PlayerPrefs.SetString(SessionKey, json);
            PlayerPrefs.Save();
            Debug.Log("[Supabase] Session saved to PlayerPrefs.");
        }
    }

    /// <summary>
    /// Loads the saved session from the device's local storage.
    /// </summary>
    public Session LoadSession()
    {
        if (PlayerPrefs.HasKey(SessionKey))
        {
            try
            {
                string json = PlayerPrefs.GetString(SessionKey);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Session>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Supabase] Failed to load session: {ex.Message}");
                return null;
            }
        }
        return null;
    }

    /// <summary>
    /// Clears the session when the user logs out.
    /// </summary>
    public void DestroySession()
    {
        PlayerPrefs.DeleteKey(SessionKey);
        PlayerPrefs.Save();
        Debug.Log("[Supabase] Session destroyed.");
    }
}
