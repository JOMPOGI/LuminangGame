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
                <html>
                <head>
                    <meta name='viewport' content='width=device-width, initial-scale=1'>
                    <style>
                        body {
                            background: radial-gradient(circle at center, #0a1020 0%, #03050a 100%);
                            color: #e0d0b0;
                            font-family: 'Cormorant Garamond', 'Georgia', serif;
                            display: flex;
                            justify-content: center;
                            align-items: center;
                            height: 100vh;
                            margin: 0;
                            overflow: hidden;
                        }
                        .container {
                            text-align: center;
                            padding: 40px;
                            background: rgba(20, 25, 40, 0.6);
                            border: 1px solid rgba(180, 150, 100, 0.3);
                            border-radius: 20px;
                            backdrop-filter: blur(10px);
                            box-shadow: 0 0 50px rgba(0, 0, 0, 0.5), 0 0 20px rgba(180, 150, 100, 0.1);
                            max-width: 400px;
                            animation: fadeIn 1.2s ease-out;
                        }
                        h1 {
                            font-size: 2.5rem;
                            letter-spacing: 4px;
                            margin-bottom: 10px;
                            color: #c0a060;
                            text-shadow: 0 2px 10px rgba(0,0,0,0.5);
                        }
                        p {
                            font-size: 1.1rem;
                            opacity: 0.8;
                            line-height: 1.6;
                        }
                        .icon {
                            font-size: 50px;
                            margin-bottom: 20px;
                            color: #c0a060;
                            display: block;
                        }
                        @keyframes fadeIn {
                            from { opacity: 0; transform: translateY(20px); }
                            to { opacity: 1; transform: translateY(0); }
                        }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h1 style='font-size: 1.8rem; letter-spacing: 2px; color: #e0d0b0;'>Authentication Successful</h1>
                        <p id='status' style='color: #c0a060; font-size: 1.3rem; margin-top: 20px; letter-spacing: 1px;'>Sending login data to the game...</p>
                    </div>
                    <script>
                        if (window.location.hash && !window.location.search) {
                            var targetUrl = 'http://localhost:54321/?' + window.location.hash.substring(1);
                            fetch(targetUrl).then(function() {
                                document.getElementById('status').innerText = 'Done! You may now return to the game.';
                            }).catch(function(err) {
                                document.getElementById('status').innerText = 'Connection error. Try logging in again.';
                            });
                        } else if (window.location.search) {
                            document.getElementById('status').innerText = 'Done! You may now return to the game.';
                        }
                    </script>
                </body>
                </html>";

            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();

            // Check if we have the query (which was originally the fragment)
            Debug.Log($"[RedirectListener] Request received. URL: {request.Url}");
            Debug.Log($"[RedirectListener] Query string count: {request.QueryString.Count}");
            foreach (string key in request.QueryString.AllKeys)
            {
                Debug.Log($"[RedirectListener] Query param: {key} = {(key == "access_token" || key == "refresh_token" ? "[HIDDEN]" : request.QueryString[key])}");
            }

            if (request.QueryString.Count > 0)
            {
                string fullUrl = request.Url.ToString();
                Debug.Log($"[RedirectListener] Passing URL to ProcessResultUrl (token hidden for security)");
                UnityMainThreadDispatcher.Enqueue(() => {
                    SupabaseManager.Instance.ProcessResultUrl(fullUrl);
                });
            }
            else
            {
                Debug.Log("[RedirectListener] First request (fragment redirect). Waiting for second request with tokens...");
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
