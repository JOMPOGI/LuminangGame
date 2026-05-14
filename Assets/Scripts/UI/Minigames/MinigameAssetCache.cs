using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Luminang.Database;

namespace Luminang.UI.Minigames
{
    /// <summary>
    /// Professional Asset Cache for Minigames.
    /// Downloads all images once during loading and keeps them in memory.
    /// </summary>
    public class MinigameAssetCache : MonoBehaviour
    {
        public static MinigameAssetCache Instance { get; private set; }

        private Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();
        private bool _isPreloading = false;
        public bool IsReady => !_isPreloading && _spriteCache.Count > 0;
        public float PreloadProgress { get; private set; } = 0f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Starts the full preloading process for all minigame assets.
        /// </summary>
        public void StartPreload()
        {
            if (_isPreloading) return;
            StartCoroutine(PreloadSequence());
        }

        private IEnumerator PreloadSequence()
        {
            _isPreloading = true;
            PreloadProgress = 0.1f; // Initial nudge

            Debug.Log("[AssetCache] Starting full minigame asset preload...");

            if (SupabaseManager.Instance == null || SupabaseManager.Instance.client == null)
            {
                Debug.LogError("[AssetCache] Cannot preload: SupabaseManager missing!");
                _isPreloading = false;
                yield break;
            }

            // 1. Fetch all phrases from Word Rush (prompts)
            var rushResponse = SupabaseManager.Instance.client.From<WordRushPromptModel>().Get();
            yield return new WaitUntil(() => rushResponse.IsCompleted);

            if (rushResponse.Result.Models.Count == 0)
            {
                Debug.LogWarning("[AssetCache] No Word Rush challenges found to preload.");
            }

            var models = rushResponse.Result.Models;
            int totalUrls = models.Count * 3; // Idle, Happy, Confused
            int downloadedCount = 0;

            foreach (var model in models)
            {
                // Download in sequence to avoid hitting rate limits, but we could parallelize later
                yield return StartCoroutine(DownloadAndCache(model.IdleImageUrl, () => {
                    downloadedCount++;
                    PreloadProgress = 0.1f + (0.9f * (float)downloadedCount / totalUrls);
                }));
                yield return StartCoroutine(DownloadAndCache(model.HappyImageUrl, () => {
                    downloadedCount++;
                    PreloadProgress = 0.1f + (0.9f * (float)downloadedCount / totalUrls);
                }));
                yield return StartCoroutine(DownloadAndCache(model.ConfusedImageUrl, () => {
                    downloadedCount++;
                    PreloadProgress = 0.1f + (0.9f * (float)downloadedCount / totalUrls);
                }));
            }

            Debug.Log($"[AssetCache] Preload complete! Cached {_spriteCache.Count} sprites.");
            _isPreloading = false;
            PreloadProgress = 1f;
        }

        private IEnumerator DownloadAndCache(string url, System.Action onComplete)
        {
            if (string.IsNullOrEmpty(url) || _spriteCache.ContainsKey(url))
            {
                onComplete?.Invoke();
                yield break;
            }

            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    Texture2D tex = DownloadHandlerTexture.GetContent(www);
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    _spriteCache[url] = sprite;
                }
                else
                {
                    Debug.LogWarning($"[AssetCache] Failed to download: {url}");
                }
            }

            onComplete?.Invoke();
        }

        /// <summary>
        /// Retrieves a cached sprite by its URL. Returns null if not cached.
        /// </summary>
        public Sprite GetSprite(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            if (_spriteCache.TryGetValue(url, out Sprite sprite))
            {
                return sprite;
            }
            return null;
        }
    }
}
