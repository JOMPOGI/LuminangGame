using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public enum RegionMode { Ilokano, Cebuano, Maranao, BossBattle }

public class PhraseEvaluator : MonoBehaviour
{
    public static PhraseEvaluator Instance { get; private set; }
    public RegionMode CurrentRegion { get; private set; } = RegionMode.BossBattle;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetRegion(RegionMode mode)
    {
        CurrentRegion = mode;
        Debug.Log($"Speech Region set to: {mode}");
    }

    public float CalculateAccuracy(string input, string target)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(target)) return 0;

        string normalizedInput = NormalizeText(input);
        string normalizedTarget = NormalizeText(target);

        // 1. Word-level similarity — F1-style (penalizes extra wrong words)
        string[] inputWords = normalizedInput.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        string[] targetWords = normalizedTarget.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        int matchCount = 0;
        foreach (var tWord in targetWords)
        {
            if (inputWords.Any(iWord => IsWordMatch(iWord, tWord)))
                matchCount++;
        }

        // SPECIAL CASE: If all target words appear IN ORDER in the input
        // ONLY applies for multi-word phrases (2+ words) to avoid false matches
        // e.g. "Ti nagan ko ket Maria Clara Santos" → target "Ti nagan ko ket" → 100%
        // But single word "mangan" inside "kaya't ko ti mangan iti taraon" → NOT 100%
        if (targetWords.Length >= 2)
        {
            int cursor = 0;
            bool inOrder = true;
            foreach (var tWord in targetWords)
            {
                bool found = false;
                while (cursor < inputWords.Length)
                {
                    if (IsWordMatch(inputWords[cursor], tWord)) { found = true; cursor++; break; }
                    cursor++;
                }
                if (!found) { inOrder = false; break; }
            }
            if (inOrder && matchCount == targetWords.Length) return 100f;
        }

        // Recall: how many target words were found
        float recall = (targetWords.Length == 0) ? 0 : ((float)matchCount / targetWords.Length);

        // Precision: of all input words, how many were actually correct
        // (penalizes random extra words like "arabi")
        float precision = (inputWords.Length == 0) ? 0 : ((float)matchCount / inputWords.Length);

        // F1 blend — but give names/extra words a slight pass (max 1 extra word allowed without penalty)
        int extraWords = Mathf.Max(0, inputWords.Length - targetWords.Length - 1);
        float penalizedPrecision = (extraWords > 0) ? precision : 1.0f * recall; // only penalize beyond 1 extra word

        float wordAccuracy = ((recall + Mathf.Min(precision, penalizedPrecision)) / 2f) * 100f;

        // 2. Character-level similarity (Sub-string focused)
        // If input is longer, we check if the target matches any part of the input
        float charAccuracy = 0;
        if (normalizedInput.Contains(normalizedTarget))
        {
            charAccuracy = 100f;
        }
        else
        {
            // Fallback to Levenshtein but only penalize based on target length
            int charDistance = LevenshteinDistance(normalizedInput, normalizedTarget);
            int denominator = normalizedTarget.Length; // Use target length to avoid penalizing extra words
            charAccuracy = (1.0f - (float)charDistance / Math.Max(denominator, normalizedInput.Length)) * 100f;
            
            // If the target is a significant part of the input, give a boost
            if (normalizedInput.Length > normalizedTarget.Length && normalizedInput.StartsWith(normalizedTarget))
            {
                charAccuracy = Mathf.Max(charAccuracy, 85f); // Partial match boost
            }
        }

        // Blend them: If word accuracy is 100%, we lean heavily on that
        if (wordAccuracy >= 99f) return 100f; 

        return Mathf.Clamp((wordAccuracy * 0.6f) + (charAccuracy * 0.4f), 0f, 100f);
    }

    private bool IsWordMatch(string inputWord, string targetWord)
    {
        if (inputWord == targetWord) return true;
        
        // Allow minor spelling errors in words (max 1-2 chars depending on length)
        int distance = LevenshteinDistance(inputWord, targetWord);
        int threshold = targetWord.Length <= 3 ? 0 : (targetWord.Length <= 6 ? 1 : 2);
        return distance <= threshold;
    }

    private string NormalizeText(string text)
    {
        // Remove punctuation and lowercase
        char[] punctuation = { '.', ',', '?', '!', '"', '\'', '(', ')', '-', '_' };
        string clean = text.ToLower();
        foreach (char p in punctuation) clean = clean.Replace(p.ToString(), "");
        
        // Phonetic Fixes: Handle common English auto-corrections from STT engines
        clean = PhoneticNormalize(clean);
        
        return clean.Trim();
    }

    private string PhoneticNormalize(string text)
    {
        // Dictionary of common English words that STT uses instead of Regional words
        string[,] fixes = {
            { "when", "wen" },     // Ilokano: Yes
            { "mega", "mga" },     // General: Plural marker
            { "hand", "haan" },    // Ilokano: No
            { "too", "tu" },       // Future marker
            { "who", "hu" },       // Maranao/Cebuano sounds
            { "eye", "ay" },       // Maranao emphasis
            { "can", "kan" }       // Maranao suffix
        };

        string result = text;
        for (int i = 0; i < fixes.GetLength(0); i++)
        {
            // Use regex or word boundaries to avoid replacing parts of words
            result = System.Text.RegularExpressions.Regex.Replace(
                result, 
                @"\b" + fixes[i, 0] + @"\b", 
                fixes[i, 1], 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );
        }
        return result;
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
        if (accuracy >= 90f) return "Perfect match! Well done!";
        if (accuracy >= 80f) return "Excellent! You passed!";
        if (accuracy >= 65f) return "Great! You're getting it.";
        if (accuracy >= 45f) return "Good effort! Try again.";
        if (accuracy >= 25f) return "Not quite, but you're close.";
        return "Keep practicing!";
    }

    /// <summary>
    /// Searches the dataset for ALL phrases present in the input.
    /// Useful for counting or long sentences.
    /// </summary>
    public List<(PhraseEntry entry, string language, float score)> FindAllMatches(string input)
    {
        var allPhrases = DatasetManager.Instance.GetAllPhrases();
        var matches = new List<(PhraseEntry entry, string language, float score)>();
        
        // Filter regional languages based on Map/Region
        List<string> regionalLangs = new List<string>();
        switch (CurrentRegion)
        {
            case RegionMode.Ilokano: regionalLangs.Add("ilokano"); break;
            case RegionMode.Cebuano: regionalLangs.Add("cebuano"); break;
            case RegionMode.Maranao: regionalLangs.Add("maranao"); break;
            case RegionMode.BossBattle: 
                regionalLangs.Add("ilokano"); 
                regionalLangs.Add("cebuano"); 
                regionalLangs.Add("maranao"); 
                break;
        }

        foreach (var entry in allPhrases)
        {
            foreach (var lang in regionalLangs)
            {
                string target = entry.GetPhrase(lang);
                if (string.IsNullOrEmpty(target) || target == "___") continue;

                float score = CalculateAccuracy(input, target);

                // Must score at least 65% to be considered a valid match
                if (score >= 65f)
                {
                    matches.Add((entry, lang, score));
                }
            }
        }

        // Sort by position in the original input string to maintain counting order
        return matches.OrderBy(m => input.ToLower().IndexOf(m.entry.GetPhrase(m.language).ToLower())).ToList();
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

        // 1. Prioritize Regional Languages based on Map/Region
        List<string> regionalLangs = new List<string>();
        switch (CurrentRegion)
        {
            case RegionMode.Ilokano: regionalLangs.Add("ilokano"); break;
            case RegionMode.Cebuano: regionalLangs.Add("cebuano"); break;
            case RegionMode.Maranao: regionalLangs.Add("maranao"); break;
            case RegionMode.BossBattle: 
                regionalLangs.Add("ilokano"); 
                regionalLangs.Add("cebuano"); 
                regionalLangs.Add("maranao"); 
                break;
        }

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
                }
            }
        }

        // 2. Check English ONLY to detect if they are speaking English instead of regional
        bool matchedEnglish = false;
        float bestEnglishScore = 0f;
        foreach (var entry in allPhrases)
        {
            float englishScore = CalculateAccuracy(input, entry.english);
            if (englishScore > bestEnglishScore)
            {
                bestEnglishScore = englishScore;
            }
        }

        // If English is a much better match than regional, flag it
        if (bestEnglishScore > 85f && bestEnglishScore > (maxScore + 15f))
        {
            matchedEnglish = true;
        }

        return (bestEntry, bestLang, maxScore, matchedEnglish);
    }
}
