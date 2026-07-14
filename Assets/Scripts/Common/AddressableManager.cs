using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Luminang.Common
{
    public class AddressableManager : MonoBehaviour
    {
        public static AddressableManager Instance { get; private set; }

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

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// Checks the download size of a label or specific key before downloading.
        /// Returns 0 if all assets are already downloaded and cached on the phone.
        /// </summary>
        /// <param name="label">The addressables label or address group (e.g., "Minigames_Assets")</param>
        /// <param name="onComplete">Callback returning size in bytes, or -1 if failed</param>
        public void GetDownloadSize(string label, Action<long> onComplete)
        {
            Addressables.GetDownloadSizeAsync(label).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    onComplete?.Invoke(handle.Result);
                }
                else
                {
                    Debug.LogError($"[Addressables] Failed to get download size for label '{label}': {handle.OperationException}");
                    onComplete?.Invoke(-1);
                }
            };
        }

        /// <summary>
        /// Starts a coroutine to download all assets associated with a specific label or address group,
        /// saving them permanently to the device cache.
        /// </summary>
        /// <param name="label">The addressables label/group to download</param>
        /// <param name="onProgress">Callback receiving download progress (0f to 1f)</param>
        /// <param name="onComplete">Callback indicating whether download succeeded</param>
        public void DownloadAssets(string label, Action<float> onProgress, Action<bool> onComplete)
        {
            StartCoroutine(DownloadAssetsCoroutine(label, onProgress, onComplete));
        }

        private IEnumerator DownloadAssetsCoroutine(string label, Action<float> onProgress, Action<bool> onComplete)
        {
            Debug.Log($"[Addressables] Checking and downloading assets for group: {label}");
            AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(label, true);

            while (!downloadHandle.IsDone)
            {
                float progress = downloadHandle.PercentComplete;
                onProgress?.Invoke(progress);
                yield return null;
            }

            bool success = downloadHandle.Status == AsyncOperationStatus.Succeeded;
            if (success)
            {
                Debug.Log($"[Addressables] Successfully downloaded and cached assets for: {label}");
                onProgress?.Invoke(1.0f);
            }
            else
            {
                Debug.LogError($"[Addressables] Download failed for: {label}. Exception: {downloadHandle.OperationException}");
            }

            Addressables.Release(downloadHandle);
            onComplete?.Invoke(success);
        }

        /// <summary>
        /// Clears all cached addressable bundles. Useful for a 'Clear Cache' option in settings.
        /// </summary>
        public void ClearCache(string label, Action<bool> onComplete)
        {
            Addressables.ClearDependencyCacheAsync(label, true).Completed += handle =>
            {
                bool success = handle.Status == AsyncOperationStatus.Succeeded;
                if (success)
                {
                    Debug.Log($"[Addressables] Successfully cleared local cache for: {label}");
                }
                else
                {
                    Debug.LogError($"[Addressables] Failed to clear local cache for: {label}");
                }
                onComplete?.Invoke(success);
            };
        }
    }
}
