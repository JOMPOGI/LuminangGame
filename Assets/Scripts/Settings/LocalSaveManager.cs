using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class GameSaveData
{
    public string lastRegionId;
    public bool isCrystalRestored;
    public List<string> unlockedVocab = new List<string>();
    public List<string> unlockedLore = new List<string>();
    public List<string> completedQuests = new List<string>();
}

public class LocalSaveManager : MonoBehaviour
{
    public static LocalSaveManager Instance { get; private set; }
    
    public GameSaveData currentSaveData;
    private string saveFilePath;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        saveFilePath = Path.Combine(Application.persistentDataPath, "luminang_local_save.json");
        LoadGame();
    }

    public void SaveGame()
    {
        if (currentSaveData == null) currentSaveData = new GameSaveData();

        // Pull data from managers
        if (JournalManager.Instance != null)
        {
            currentSaveData.unlockedVocab = JournalManager.Instance.unlockedVocabulary;
            currentSaveData.unlockedLore = JournalManager.Instance.unlockedLore;
        }

        string json = JsonUtility.ToJson(currentSaveData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"[LocalSaveManager] Game saved locally to: {saveFilePath}");
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            currentSaveData = JsonUtility.FromJson<GameSaveData>(json);
            Debug.Log($"[LocalSaveManager] Game loaded locally.");
        }
        else
        {
            currentSaveData = new GameSaveData();
            Debug.Log("[LocalSaveManager] No local save found. Created new profile.");
        }

        // Push data to managers
        if (JournalManager.Instance != null)
        {
            JournalManager.Instance.LoadJournalData(currentSaveData.unlockedVocab, currentSaveData.unlockedLore);
        }
    }
}
