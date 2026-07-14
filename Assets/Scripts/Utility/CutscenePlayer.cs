using System.Collections;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Plays an optional cut‑scene video. If no video is assigned it simply
/// waits a short fallback duration so the gameplay flow does not stall.
/// </summary>
[RequireComponent(typeof(VideoPlayer))]
public class CutscenePlayer : MonoBehaviour
{
    [Tooltip("Assign a VideoClip for the cutscene. Leave empty for fallback pause.")]
    public VideoClip videoClip;

    // Fallback wait time in seconds when no video is set.
    public float fallbackDuration = 2f;

    private VideoPlayer _vp;

    private void Awake()
    {
        _vp = GetComponent<VideoPlayer>();
        _vp.playOnAwake = false;
        _vp.waitForFirstFrame = true;
        _vp.isLooping = false;
    }

    /// <summary>
    /// Play the assigned video or wait for the fallback duration.
    /// Use via: <c>yield return StartCoroutine(cutscenePlayer.Play());</c>
    /// </summary>
    public IEnumerator Play()
    {
        if (videoClip != null)
        {
            _vp.clip = videoClip;
            _vp.Prepare();
            while (!_vp.isPrepared) yield return null;
            _vp.Play();
            while (_vp.isPlaying) yield return null;
        }
        else
        {
            Debug.Log("[CutscenePlayer] No video assigned – using fallback pause.");
            yield return new WaitForSeconds(fallbackDuration);
        }
    }
}
