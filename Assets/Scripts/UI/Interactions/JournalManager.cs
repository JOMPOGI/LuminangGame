using UnityEngine;
using System.Collections.Generic;

public class JournalManager : MonoBehaviour
{
    public static JournalManager Instance { get; private set; }

    [Header("Journal Data")]
    public List<string> unlockedVocabulary = new List<string>();
    public List<string> unlockedLore = new List<string>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UnlockVocabulary(string vocabId)
    {
        if (!unlockedVocabulary.Contains(vocabId))
        {
            unlockedVocabulary.Add(vocabId);
            Debug.Log($"[JournalManager] Unlocked Vocabulary: {vocabId}");
            
            // Auto-save if LocalSaveManager exists
            if (LocalSaveManager.Instance != null)
            {
                LocalSaveManager.Instance.SaveGame();
            }
        }
    }

    public void UnlockLore(string loreId)
    {
        if (!unlockedLore.Contains(loreId))
        {
            unlockedLore.Add(loreId);
            Debug.Log($"[JournalManager] Unlocked Lore: {loreId}");
            
            if (LocalSaveManager.Instance != null)
            {
                LocalSaveManager.Instance.SaveGame();
            }
        }
    }

    public void LoadJournalData(List<string> vocab, List<string> lore)
    {
        unlockedVocabulary = vocab ?? new List<string>();
        unlockedLore = lore ?? new List<string>();
    }
}
