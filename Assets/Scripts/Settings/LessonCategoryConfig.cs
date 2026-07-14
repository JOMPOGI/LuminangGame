using System.Collections.Generic;

/// <summary>
/// Hardcoded configuration mappings for Lesson Categories in the game.
/// Provides user-facing names, lesson numbers, and detailed descriptions
/// so that the reusable LessonIntroPanel can show descriptive previews.
/// </summary>
public class LessonCategoryConfig
{
    public string categoryName; // database identifier/key
    public string categoryDisplayName;
    public string lessonDisplayName;
    public int categoryNumber;
    public int lessonNumber;
    public string lessonDescription;

    private static readonly Dictionary<string, LessonCategoryConfig> ConfigMap = new Dictionary<string, LessonCategoryConfig>
    {
        {
            "Greetings", new LessonCategoryConfig
            {
                categoryName = "Greetings",
                categoryDisplayName = "Conversational & Social Expressions",
                lessonDisplayName = "Greetings",
                categoryNumber = 1,
                lessonNumber = 1,
                lessonDescription = "Greetings are the foundation of every conversation. Throughout your journey, you'll meet merchants, travelers, elders, children, and many other members of the community. Learning how to greet them properly will help you communicate naturally with everyone you meet. Master these expressions before continuing your adventure."
            }
        },
        {
            "Gratitude", new LessonCategoryConfig
            {
                categoryName = "Gratitude",
                categoryDisplayName = "Conversational & Social Expressions",
                lessonDisplayName = "Gratitude & Respect",
                categoryNumber = 1,
                lessonNumber = 2,
                lessonDescription = "Showing gratitude and respect builds deep connections within the community. Learn how to thank people and show proper respect to the elders and hosts of this region."
            }
        }
    };

    /// <summary>
    /// Finds config for a given category name. Returns null if not found.
    /// </summary>
    public static LessonCategoryConfig Find(string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName)) return null;
        ConfigMap.TryGetValue(categoryName, out var config);
        return config;
    }
}
