using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class GroqWhisperManager : MonoBehaviour
{
    public static GroqWhisperManager Instance { get; private set; }

    private string _apiKey;
    private const string GROQ_WHISPER_URL = "https://api.groq.com/openai/v1/audio/transcriptions";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadApiKey();
    }

    private void LoadApiKey()
    {
        TextAsset config = Resources.Load<TextAsset>("GroqConfig");
        if (config != null)
        {
            _apiKey = config.text.Trim();
        }
        else
        {
            Debug.LogError("GroqConfig.txt not found in Resources!");
        }
    }

    public void Transcribe(string filePath, Action<string> onSuccess, Action<string> onError)
    {
        StartCoroutine(TranscribeCoroutine(filePath, onSuccess, onError));
    }

    private IEnumerator TranscribeCoroutine(string filePath, Action<string> onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(_apiKey) || _apiKey == "YOUR_GROQ_API_KEY_HERE")
        {
            onError?.Invoke("API Key not set!");
            yield break;
        }

        byte[] audioData = File.ReadAllBytes(filePath);
        
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", audioData, "speech.wav", "audio/wav");
        form.AddField("model", "whisper-large-v3"); // Groq's supported Whisper model
        form.AddField("response_format", "json");

        using (UnityWebRequest request = UnityWebRequest.Post(GROQ_WHISPER_URL, form))
        {
            request.SetRequestHeader("Authorization", "Bearer " + _apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<WhisperResponse>(request.downloadHandler.text);
                onSuccess?.Invoke(response.text);
            }
            else
            {
                Debug.LogError($"Groq API Error: {request.error}\n{request.downloadHandler.text}");
                onError?.Invoke(request.error);
            }
        }
    }

    [Serializable]
    private class WhisperResponse
    {
        public string text;
    }
}
