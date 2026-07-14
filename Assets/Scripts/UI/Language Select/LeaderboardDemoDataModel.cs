using System;
using System.Collections.Generic;

[Serializable]
public class LeaderboardDemoData
{
    public List<LeaderboardPlayer> players;
}

[Serializable]
public class LeaderboardPlayer
{
    public string username;
    public string picture;                   // Path to sprite or null
    public int ilokano_lessons_completed;    // Out of TOTAL_LESSONS
    public int cebuano_lessons_completed;    // Out of TOTAL_LESSONS
    public int clothing_items_bought;
    public int coins;
    public string last_active;
    public bool is_current_player;

    // Computed at runtime by LeaderboardManager — NOT stored in JSON
    [NonSerialized] public float ilokano_progress;   // 0–100
    [NonSerialized] public float cebuano_progress;   // 0–100
    [NonSerialized] public float overall_progress;   // 0–100
}
