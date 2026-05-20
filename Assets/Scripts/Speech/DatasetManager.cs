using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class PhraseEntry
{
    public string category;
    public string english;
    public string ilokano;
    public string cebuano;

    public string GetPhrase(string language)
    {
        switch (language.ToLower())
        {
            case "english": return english;
            case "ilokano": return ilokano;
            case "cebuano": return cebuano;
            default: return english;
        }
    }
}

[Serializable]
public class PhraseDataset
{
    public List<PhraseEntry> phrases;
}

public class DatasetManager : MonoBehaviour
{
    public static DatasetManager Instance { get; private set; }

    [SerializeField] private TextAsset datasetJson;
    private PhraseDataset dataset;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadDataset();
    }

    private void LoadDataset()
    {
        if (datasetJson == null)
        {
            datasetJson = Resources.Load<TextAsset>("LuminangPhrases");
        }

        if (datasetJson != null)
        {
            string jsonText = datasetJson.text;
            
            // Clean up the JSON string (Remove BOM and hidden characters)
            jsonText = jsonText.Trim();
            if (jsonText.Length > 0 && jsonText[0] != '{')
            {
                jsonText = jsonText.Substring(jsonText.IndexOf('{'));
            }

            try 
            {
                dataset = JsonUtility.FromJson<PhraseDataset>(jsonText);
                Debug.Log($"Dataset successfully loaded with {dataset.phrases.Count} phrases.");
            }
            catch (Exception e)
            {
                Debug.LogError($"JSON Parsing Failed! Error: {e.Message}");
                Debug.LogError($"JSON Content Start: {jsonText.Substring(0, Mathf.Min(jsonText.Length, 100))}");
            }
        }
        else
        {
            Debug.LogError("LuminangPhrases.json not found in Resources!");
        }
    }


    public List<PhraseEntry> GetAllPhrases() => dataset.phrases;
    
    public PhraseEntry GetPhraseByEnglish(string englishText)
    {
        return dataset.phrases.FirstOrDefault(p => p.english.Equals(englishText, StringComparison.OrdinalIgnoreCase));
    }
}
