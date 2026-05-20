using UnityEngine;
using System.Collections.Generic;

namespace Luminang.UI.Minigames
{
    [CreateAssetMenu(fileName = "NewMatchingLesson", menuName = "Luminang/Minigames/Matching Lesson")]
    public class MatchingLessonData : ScriptableObject
    {
        [Header("Lesson Info")]
        public string lessonTitle;
        [TextArea(2, 5)]
        public string lessonDescription;

        [Header("Pairs")]
        public List<MatchingPair> pairs;
    }

    [System.Serializable]
    public class MatchingPair
    {
        public Sprite image;
        public string correctWord;
        
        [Tooltip("Unique ID to link the pair, usually the same as the correct word")]
        public string pairID;
    }
}
