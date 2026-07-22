using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Luminang.Database;

namespace Luminang.UI.Minigames
{
    public class CurriculumManager : MonoBehaviour
    {
        public static CurriculumManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Fetches all active languages from the database.
        /// </summary>
        public async Task<List<LanguageModel>> GetLanguages()
        {
            var response = await SupabaseManager.Instance.client
                .From<LanguageModel>()
                .Get();
            
            return response.Models;
        }

        /// <summary>
        /// Fetches all lesson categories (Greetings, Numbers, etc.).
        /// </summary>
        public async Task<List<LessonCategoryModel>> GetCategories()
        {
            var response = await SupabaseManager.Instance.client
                .From<LessonCategoryModel>()
                .Get();
            
            return response.Models;
        }

        /// <summary>
        /// Fetches all vocabulary items and their translations for a specific category and language.
        /// This is the main data source for your matching game.
        /// </summary>
        public async Task<List<VocabularyTranslationModel>> GetLessonVocabulary(string categoryId, int languageId)
        {
            // We query the translations table and "join" the vocabulary base data
            var response = await SupabaseManager.Instance.client
                .From<VocabularyTranslationModel>()
                .Filter("language_id", Postgrest.Constants.Operator.Equals, languageId)
                // We only want translations for vocabulary in the selected category
                // This part usually requires a subquery or a careful join in Postgrest
                .Get();

            // Since Postgrest C# joins can be tricky with specific filters, 
            // we'll filter the category on the client side for now, 
            // or use a more advanced Select if your Supabase schema allows it.
            var filtered = response.Models.Where(t => t.Vocabulary != null && t.Vocabulary.CategoryId == categoryId).ToList();
            
            return filtered;
        }

        /// <summary>
        /// A more direct way to get matching pairs for the game.
        /// </summary>
        public async Task<List<MatchingPairData>> GetMatchingPairs(string categoryId, int languageId)
        {
            var translations = await GetLessonVocabulary(categoryId, languageId);
            
            List<MatchingPairData> pairs = new List<MatchingPairData>();
            foreach (var t in translations)
            {
                pairs.Add(new MatchingPairData
                {
                    id = t.VocabularyId,
                    translatedText = t.TranslatedText,
                    englishTerm = t.Vocabulary.EnglishTerm,
                    iconPath = t.Vocabulary.IconUrl,
                    illustrationPath = t.Vocabulary.IllustrationUrl,
                    audioPath = t.AudioUrl
                });
            }
            return pairs;
        }
    }

    [System.Serializable]
    public class MatchingPairData
    {
        public string id;
        public string translatedText;
        public string englishTerm;
        public string iconPath;
        public string illustrationPath;
        public string audioPath;
    }
}
