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
        form.AddField("model", "whisper-large-v3"); 
        form.AddField("response_format", "json");

        // Add a prompt to help Whisper with regional language vocabulary
        // We filter based on the current Map/Region to maximize accuracy.
        var phrases = DatasetManager.Instance.GetAllPhrases();
        var region = PhraseEvaluator.Instance.CurrentRegion;
        
        System.Collections.Generic.List<string> promptWords = new System.Collections.Generic.List<string> { 
            "mga", "po", "kabsat", "philippines"
        };
        
        for (int i = 0; i < phrases.Count && promptWords.Count < 50; i++)
        {
            if (region == RegionMode.Ilokano || region == RegionMode.BossBattle)
            {
                if (!string.IsNullOrEmpty(phrases[i].ilokano) && phrases[i].ilokano != "___") promptWords.Add(phrases[i].ilokano);
            }
            if (region == RegionMode.Cebuano || region == RegionMode.BossBattle)
            {
                if (!string.IsNullOrEmpty(phrases[i].cebuano) && phrases[i].cebuano != "___") promptWords.Add(phrases[i].cebuano);
            }
            if (region == RegionMode.Maranao || region == RegionMode.BossBattle)
            {
                if (!string.IsNullOrEmpty(phrases[i].maranao) && phrases[i].maranao != "___") promptWords.Add(phrases[i].maranao);
            }
        }

        string lexiconPrompt = string.Join(", ", promptWords.ToArray());
        form.AddField("prompt", lexiconPrompt);

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
