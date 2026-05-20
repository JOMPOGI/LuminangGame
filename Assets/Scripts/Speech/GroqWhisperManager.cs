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

    public void Transcribe(string filePath, Action<string> onSuccess, Action<string> onError)
    {
        StartCoroutine(TranscribeCoroutine(filePath, onSuccess, onError));
    }

    private IEnumerator TranscribeCoroutine(string filePath, Action<string> onSuccess, Action<string> onError)
    {
        if (!File.Exists(filePath))
        {
            onError?.Invoke("Audio file not found!");
            yield break;
        }

        byte[] audioData = File.ReadAllBytes(filePath);
        
        WWWForm form = new WWWForm();
        form.AddBinaryData("audio", audioData, "speech.wav", "audio/wav");

        using (UnityWebRequest request = UnityWebRequest.Post(GROQ_WHISPER_URL, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<WhisperResponse>(request.downloadHandler.text);
                onSuccess?.Invoke(response.text);
            }
            else
            {
                Debug.LogError($"Whisper Backend Error: {request.error}\n{request.downloadHandler.text}");
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
