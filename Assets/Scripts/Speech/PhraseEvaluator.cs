using System;
using UnityEngine;

public class PhraseEvaluator : MonoBehaviour
{
    public static PhraseEvaluator Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public float CalculateAccuracy(string input, string target)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(target)) return 0;

        string normalizedInput = NormalizeText(input);
        string normalizedTarget = NormalizeText(target);

        int distance = LevenshteinDistance(normalizedInput, normalizedTarget);
        int maxLength = Math.Max(normalizedInput.Length, normalizedTarget.Length);

        if (maxLength == 0) return 100f;

        float accuracy = (1.0f - (float)distance / maxLength) * 100f;
        return Mathf.Clamp(accuracy, 0f, 100f);
    }

    private string NormalizeText(string text)
    {
        return text.ToLower().Trim();
    }

    private int LevenshteinDistance(string s, string t)
    {
        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        if (n == 0) return m;
        if (m == 0) return n;

        for (int i = 0; i <= n; d[i, 0] = i++) ;
        for (int j = 0; j <= m; d[0, j] = j++) ;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }
        return d[n, m];
    }

    public string GetFeedback(float accuracy)
    {
        if (accuracy >= 90f) return "Excellent";
        if (accuracy >= 75f) return "Good";
        return "Needs Practice";
    }

    /// <summary>
    /// Searches the dataset for the closest matching phrase.
    /// Returns the matched entry and the accuracy score.
    /// </summary>
    public (PhraseEntry entry, string language, float score, bool isEnglish) FindBestMatch(string input)
    {
        var allPhrases = DatasetManager.Instance.GetAllPhrases();
        PhraseEntry bestEntry = null;
        string bestLang = "";
        float maxScore = -1f;
        bool matchedEnglish = false;

        // 1. First, search for Regional Languages
        string[] regionalLangs = { "ilokano", "cebuano", "maranao" };
        foreach (var entry in allPhrases)
        {
            foreach (var lang in regionalLangs)
            {
                string target = entry.GetPhrase(lang);
                if (string.IsNullOrEmpty(target) || target == "___") continue;

                float score = CalculateAccuracy(input, target);
                if (score > maxScore)
                {
                    maxScore = score;
                    bestEntry = entry;
                    bestLang = lang;
                    matchedEnglish = false;
                }
            }
        }

        // 2. If no strong regional match, check if they said the English version
        if (maxScore < 75f) 
        {
            foreach (var entry in allPhrases)
            {
                float englishScore = CalculateAccuracy(input, entry.english);
                if (englishScore > 85f && englishScore > maxScore)
                {
                    maxScore = englishScore;
                    bestEntry = entry;
                    bestLang = "english";
                    matchedEnglish = true;
                }
            }
        }

        return (bestEntry, bestLang, maxScore, matchedEnglish);
    }
}
