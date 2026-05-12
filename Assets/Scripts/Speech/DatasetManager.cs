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
    public string maranao;

    public string GetPhrase(string language)
    {
        switch (language.ToLower())
        {
            case "english": return english;
            case "ilokano": return ilokano;
            case "cebuano": return cebuano;
            case "maranao": return maranao;
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
    private HashSet<string> lexicon;

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
                BuildLexicon();
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
            Debug.LogError("LuminangDataset.json not found in Resources!");
        }
    }

    private void BuildLexicon()
    {
        lexicon = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in dataset.phrases)
        {
            AddPhraseToLexicon(entry.english);
            AddPhraseToLexicon(entry.ilokano);
            AddPhraseToLexicon(entry.cebuano);
            AddPhraseToLexicon(entry.maranao);
        }
    }

    private void AddPhraseToLexicon(string phrase)
    {
        if (string.IsNullOrEmpty(phrase)) return;

        // Split by whitespace and remove special characters like ?, !, /
        string[] words = phrase.Split(new[] { ' ', '/', '?' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in words)
        {
            string cleanWord = word.Trim().ToLower();
            if (cleanWord != "___")
            {
                lexicon.Add(cleanWord);
            }
        }
    }

    public bool IsWordInLexicon(string word)
    {
        return lexicon.Contains(word.ToLower().Trim());
    }

    public List<PhraseEntry> GetAllPhrases() => dataset.phrases;
    
    public PhraseEntry GetPhraseByEnglish(string englishText)
    {
        return dataset.phrases.FirstOrDefault(p => p.english.Equals(englishText, StringComparison.OrdinalIgnoreCase));
    }
}
