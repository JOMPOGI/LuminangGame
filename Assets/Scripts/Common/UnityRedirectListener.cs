using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Supabase.Gotrue;

/// <summary>
/// PURPOSE: This script is the "Ear" of your game. It listens for the login 
/// response from the browser and sends the login tokens back to Unity.
/// 
/// CROSS-PLATFORM SUPPORT:
/// 1. In the UNITY EDITOR: It starts a tiny local web server on port 54321.
/// 2. On MOBILE (Android/iOS): It listens for the "luminang://" deep link.
/// </summary>
public class UnityRedirectListener : MonoBehaviour
{
    private HttpListener _listener;
    private bool _isListening = false;

    public static UnityRedirectListener Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // On Mobile, we listen for Deep Links
            Application.deepLinkActivated += OnDeepLinkActivated;
            
            // If the game started FROM a deep link, handle it immediately
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                OnDeepLinkActivated(Application.absoluteURL);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Starts the local web server (For Editor Testing).
    /// </summary>
    public void StartEditorListener()
    {
#if UNITY_EDITOR
        if (_isListening) return;

        try
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:54321/");
            _listener.Start();
            _isListening = true;
            
            Debug.Log("[RedirectListener] Local server started on http://localhost:54321/");
            Task.Run(ListenForResponse);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[RedirectListener] Failed to start: {ex.Message}");
        }
#endif
    }

    private async Task ListenForResponse()
    {
#if UNITY_EDITOR
        while (_isListening)
        {
            var context = await _listener.GetContextAsync();
            var request = context.Request;
            
            // WE NEED JAVASCRIPT: Browsers don't send the '#fragment' to the server.
            // This script reloads the page once, moving the token into the '?query' so we can read it.
            string responseString = @"
                <html><body>
                <script>
                    if (window.location.hash && !window.location.search) {
                        window.location.search = window.location.hash.substring(1);
                    } else {
                        document.body.innerHTML = '<h2>Success! You can now return to Luminang.</h2>';
                    }
                </script>
                </body></html>";

            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();

            // Check if we have the query (which was originally the fragment)
            if (request.QueryString.Count > 0)
            {
                string fullUrl = request.Url.ToString();
                UnityMainThreadDispatcher.Enqueue(() => {
                    SupabaseManager.Instance.ProcessResultUrl(fullUrl);
                });
            }
        }
#endif
    }

    private void OnDeepLinkActivated(string url)
    {
        Debug.Log($"[RedirectListener] Deep Link Activated: {url}");
        UnityMainThreadDispatcher.Enqueue(() => {
            SupabaseManager.Instance.ProcessResultUrl(url);
        });
    }

    private void OnDestroy()
    {
        _isListening = false;
        _listener?.Close();
        Application.deepLinkActivated -= OnDeepLinkActivated;
    }
}
