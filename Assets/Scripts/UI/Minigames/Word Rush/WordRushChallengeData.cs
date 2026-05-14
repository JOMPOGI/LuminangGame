using UnityEngine;
using System.Collections.Generic;

namespace Luminang.UI.Minigames
{
    [System.Serializable]
    public class RushPrompt
    {
        public string id;
        public string clueText;
        public string targetPhrase;
        public string happyFeedback;
        public string confusedFeedback;
        
        [Header("Remote Assets (Optional)")]
        public string idleImageUrl;
        public string happyImageUrl;
        public string confusedImageUrl;

        [Header("Local Assets (Backup)")]
        public Sprite idleSprite;
        public Sprite happySprite;
        public Sprite confusedSprite;
    }

    [CreateAssetMenu(fileName = "NewRushChallenge", menuName = "Luminang/Minigames/Word Rush Challenge")]
    public class WordRushChallengeData : ScriptableObject
    {
        public string categoryName;
        public List<RushPrompt> prompts = new List<RushPrompt>();
    }
}
