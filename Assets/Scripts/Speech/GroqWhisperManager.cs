#pragma warning disable 0649
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class GroqWhisperManager : MonoBehaviour
{
    public static GroqWhisperManager Instance { get; private set; }

    private string _apiKey;
    private const string GROQ_WHISPER_URL = "https://luminang-nlp-service.onrender.com/transcribe";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Transcribe(string filePath, Action<string> onSuccess, Action<string> onError, string prompt = "", string language = "")
    {
        StartCoroutine(TranscribeCoroutine(filePath, onSuccess, onError, prompt, language));
    }

    private IEnumerator TranscribeCoroutine(string filePath, Action<string> onSuccess, Action<string> onError, string prompt = "", string language = "")
    {
        if (!File.Exists(filePath))
        {
            onError?.Invoke("Audio file not found!");
            yield break;
        }

        byte[] audioData = File.ReadAllBytes(filePath);
        
        WWWForm form = new WWWForm();
        form.AddBinaryData("audio", audioData, "speech.wav", "audio/wav");
        if (!string.IsNullOrEmpty(prompt))
        {
            form.AddField("prompt", prompt);
        }
        if (!string.IsNullOrEmpty(language))
        {
            form.AddField("language", language);
        }

        using (UnityWebRequest request = UnityWebRequest.Post(GROQ_WHISPER_URL, form))
        {
            request.timeout = 30; // 30 seconds timeout for mobile data

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                onError?.Invoke("No internet connection. Please check your data or Wi-Fi.");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<WhisperResponse>(request.downloadHandler.text);
                onSuccess?.Invoke(response.text);
            }
            else
            {
                string errorMessage = request.error;
                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    errorMessage = "Network error. Please check your connection.";
                }
                else if (request.result == UnityWebRequest.Result.ProtocolError)
                {
                    errorMessage = "API error. Please try again later.";
                }

                Debug.LogError($"Whisper Backend Error: {errorMessage}\n{request.downloadHandler.text}");
                onError?.Invoke(errorMessage);
            }
        }
    }


    [Serializable]
    private class WhisperResponse
    {
        public string text;
    }
}
