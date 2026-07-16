using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Loads the leaderboard demo data, sorts the players by overall progress,
/// spawns the top 10 rows in the ScrollView, manages the 'Your Rank' row at the bottom,
/// and handles clicks to update the Details panel.
/// </summary>
public class LeaderboardManager : MonoBehaviour
{
    [Header("Data Source")]
    [Tooltip("Drag the LeaderboardDemoData.json here.")]
    public TextAsset leaderboardJsonFile;

    [Header("List Settings (LeftGroup)")]
    public Transform listContentParent;
    public GameObject leaderboardRowPrefab;

    [Header("Your Rank Footer")]
    [Tooltip("The LeaderboardRowItem prefab/object used for the 'Your Rank' panel at the bottom.")]
    public LeaderboardRowItem yourRankRow;

    [Header("Details Panel (RightGroup)")]
    public LeaderboardDetailsManager detailsManager;

    /// <summary>Total number of lessons per language. Change this if the lesson count changes.</summary>
    private const int TOTAL_LESSONS = 12;

    private LeaderboardDemoData _leaderboardData;
    private List<LeaderboardPlayer> _sortedPlayers = new List<LeaderboardPlayer>();
    private LeaderboardRowItem _selectedRow; // Tracks the currently highlighted row

    private void Start()
    {
        LoadData();
        ComputeProgressFromLessons();
        SortAndDisplay();
    }

    private void LoadData()
    {
        if (leaderboardJsonFile != null)
        {
            _leaderboardData = JsonUtility.FromJson<LeaderboardDemoData>(leaderboardJsonFile.text);
            if (_leaderboardData == null || _leaderboardData.players == null)
                Debug.LogError("[LeaderboardManager] Failed to parse JSON!");
        }
        else
        {
            Debug.LogError("[LeaderboardManager] JSON file is not assigned!");
        }
    }

    private void ComputeProgressFromLessons()
    {
        if (_leaderboardData?.players == null) return;

        foreach (var player in _leaderboardData.players)
        {
            player.ilokano_progress  = (player.ilokano_lessons_completed  / (float)TOTAL_LESSONS) * 100f;
            player.cebuano_progress  = (player.cebuano_lessons_completed  / (float)TOTAL_LESSONS) * 100f;
            player.overall_progress  = (player.ilokano_progress + player.cebuano_progress) / 2f;
        }
    }

    private void SortAndDisplay()
    {
        if (_leaderboardData == null || _leaderboardData.players == null) return;

        // ---------------------------------------------------------
        // LEADERBOARD TIE-BREAKER RULES:
        // If two players have the same score, we use tie-breakers 
        // to determine who gets the higher rank:
        // 1. Highest Overall Progress wins
        // 2. Most Clothing Items Bought wins
        // 3. Most Coins wins
        // 4. Alphabetical by Username (A to Z) as a final fallback
        // ---------------------------------------------------------
        _sortedPlayers = new List<LeaderboardPlayer>(_leaderboardData.players);
        _sortedPlayers.Sort((a, b) => 
        {
            // 1. Overall Progress (Descending)
            int result = b.overall_progress.CompareTo(a.overall_progress);
            if (result != 0) return result;

            // 2. Clothing Items Bought (Descending)
            result = b.clothing_items_bought.CompareTo(a.clothing_items_bought);
            if (result != 0) return result;

            // 3. Coins (Descending)
            result = b.coins.CompareTo(a.coins);
            if (result != 0) return result;

            // 4. Username (Ascending / Alphabetical)
            return a.username.CompareTo(b.username);
        });

        // Clear existing rows
        foreach (Transform child in listContentParent)
        {
            Destroy(child.gameObject);
        }

        // Spawn Top 10
        int topCount = Mathf.Min(10, _sortedPlayers.Count);
        LeaderboardRowItem firstRow = null;

        for (int i = 0; i < topCount; i++)
        {
            GameObject newRow = Instantiate(leaderboardRowPrefab, listContentParent, false);
            LeaderboardRowItem rowScript = newRow.GetComponent<LeaderboardRowItem>();
            if (rowScript != null)
            {
                rowScript.Setup(_sortedPlayers[i], i + 1, this);
                if (i == 0) firstRow = rowScript;
            }
        }

        // Find Current Player (Luminang) for the 'Your Rank' footer
        int currentPlayerRank = -1;
        LeaderboardPlayer currentPlayer = null;

        for (int i = 0; i < _sortedPlayers.Count; i++)
        {
            if (_sortedPlayers[i].is_current_player)
            {
                currentPlayerRank = i + 1;
                currentPlayer = _sortedPlayers[i];
                break;
            }
        }

        // Setup the 'Your Rank' footer row
        if (yourRankRow != null)
        {
            if (currentPlayer != null && currentPlayerRank != -1)
            {
                yourRankRow.gameObject.SetActive(true);
                yourRankRow.Setup(currentPlayer, currentPlayerRank, this);
            }
            else
            {
                yourRankRow.gameObject.SetActive(false);
            }
        }

        // Default select the top 1 player details
        if (firstRow != null && detailsManager != null)
        {
            SelectRow(firstRow, _sortedPlayers[0], 1);
        }
    }

    public void SelectPlayer(LeaderboardPlayer player, int rank)
    {
        // Find the row that was clicked and select it
        // The row calls this directly, passing itself via OnClick — we need to route through SelectRow
        // Rows call SelectPlayer; we find the calling row via the passed LeaderboardRowItem in the overload
        if (detailsManager != null)
            detailsManager.DisplayPlayerDetails(player, rank);
    }

    /// <summary>Called by LeaderboardRowItem.OnClick — selects the row visually and updates details.</summary>
    public void SelectRow(LeaderboardRowItem clickedRow, LeaderboardPlayer player, int rank)
    {
        // Deselect previous row
        if (_selectedRow != null)
            _selectedRow.SetSelected(false);

        // Select new row
        _selectedRow = clickedRow;
        _selectedRow.SetSelected(true);

        if (detailsManager != null)
            detailsManager.DisplayPlayerDetails(player, rank);
    }
}
