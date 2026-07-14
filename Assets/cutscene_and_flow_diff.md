# Diff for CutscenePlayer.cs (new file)
```diff
+using System.Collections;
+using UnityEngine;
+using UnityEngine.Video;
+
+/// <summary>
+/// Plays an optional cut‑scene video. If no video is assigned it simply
+/// waits a short fallback duration so the gameplay flow does not stall.
+/// </summary>
+[RequireComponent(typeof(VideoPlayer))]
+public class CutscenePlayer : MonoBehaviour
+{
+    // Assign a video clip in the inspector; leave empty for placeholder.
+    [Tooltip("Leave empty to use a fallback (no video).")]
+    public VideoClip videoClip;
+
+    // How long to wait when there is no video (seconds).
+    public float fallbackDuration = 2f;
+
+    private VideoPlayer _vp;
+
+    private void Awake()
+    {
+        _vp = GetComponent<VideoPlayer>();
+        _vp.playOnAwake = false;
+        _vp.waitForFirstFrame = true;
+        _vp.isLooping = false;
+    }
+
+    /// <summary>
+    /// Call from a coroutine with:  `yield return StartCoroutine(cutscenePlayer.Play());`
+    /// </summary>
+    public IEnumerator Play()
+    {
+        if (videoClip != null)
+        {
+            _vp.clip = videoClip;
+            _vp.Prepare();
+            while (!_vp.isPrepared) yield return null;
+            _vp.Play();
+            while (_vp.isPlaying) yield return null;
+        }
+        else
+        {
+            Debug.Log("[CutscenePlayer] No video assigned – using fallback pause.");
+            yield return new WaitForSeconds(fallbackDuration);
+        }
+    }
+}
+```
+
+# Diff for RegionFlowController.cs (new file or update)
```diff
+using System.Collections;
+using UnityEngine;
+
+public enum RegionMode { Cebu, Ilocos, BossBattle }
+
+/// <summary>
+/// Orchestrates the full region experience: cut‑scene → lesson → STT → minigame → quest → crystal trial.
+/// It re‑uses the existing managers (LessonManager, MinigameManager, JournalManager, ObjectiveManager).
+/// </summary>
+public class RegionFlowController : MonoBehaviour
+{
+    [Header("Managers")]
+    [SerializeField] private LessonManager lessonManager;
+    [SerializeField] private MinigameManager minigameManager;
+    [SerializeField] private JournalManager journalManager;
+    [SerializeField] private ObjectiveManager objectiveManager;
+
+    // Optional reference to the CutscenePlayer (set in inspector).
+    [SerializeField] private CutscenePlayer cutscenePlayer;
+
+    private Coroutine _flowRoutine;
+
+    public void StartRegion(RegionMode mode)
+    {
+        if (_flowRoutine != null) StopCoroutine(_flowRoutine);
+        _flowRoutine = StartCoroutine(RegionFlowRoutine(mode));
+    }
+
+    private IEnumerator RegionFlowRoutine(RegionMode mode)
+    {
+        // 1️⃣ Play intro cut‑scene (or fallback pause).
+        if (cutscenePlayer != null)
+            yield return cutscenePlayer.StartCoroutine(cutscenePlayer.Play());
+        else
+            yield return new WaitForSeconds(1f);
+
+        // 2️⃣ Load lesson data for the region.
+        var lesson = lessonManager.LoadLessonForRegion(mode);
+        if (lesson == null) yield break;
+
+        // 3️⃣ Show lesson UI.
+        lessonManager.ShowLessonPanel(lesson);
+
+        // 4️⃣ Wait for STT success – the STTDialogueAdapter raises a static event.
+        bool sttSuccess = false;
+        void OnSTTResult(bool success, string _)
+        {
+            sttSuccess = success;
+        }
+        STTDialogueAdapter.OnSTTResult += OnSTTResult;
+        while (!sttSuccess) yield return null;
+        STTDialogueAdapter.OnSTTResult -= OnSTTResult;
+
+        // 5️⃣ Advance the guide dialogue (safe‑guard; often already called).
+        DialogueManager.Instance?.AdvanceDialogue();
+
+        // 6️⃣ Start the designated minigame.
+        if (!string.IsNullOrEmpty(lesson.minigameName))
+            yield return minigameManager.StartMinigameRoutine(lesson.minigameName);
+
+        // 7️⃣ Trigger the story quest.
+        objectiveManager.StartQuestForLesson(lesson);
+
+        // 8️⃣ Crystal trial – placeholder coroutine (replace with real implementation).
+        yield return PlayCrystalTrial(mode);
+
+        // 9️⃣ Update the journal.
+        journalManager.UpdateJournalForLesson(lesson);
+
+        Debug.Log($"[RegionFlowController] Completed flow for {mode}");
+    }
+
+    private IEnumerator PlayCrystalTrial(RegionMode mode)
+    {
+        // Placeholder – wait a couple of seconds so the flow continues.
+        Debug.Log($"[RegionFlowController] Playing crystal trial for {mode}");
+        yield return new WaitForSeconds(2f);
+    }
+}
+```
+
+---
+**How to apply these changes**
+1. In Unity, create the folder `Assets/Scripts/Utility/` if it does not exist.
+2. Add a new C# script named `CutscenePlayer.cs` and paste the content from the first diff block.
+3. In the appropriate UI/Dialogue folder (e.g., `Assets/Scripts/UI/Dialogue/`) create `RegionFlowController.cs` and paste the second diff block.
+4. Drag the `RegionFlowController` component onto a persistent GameObject in your scene (e.g., an empty `RegionFlow_GO`).
+5. Assign the existing manager references (LessonManager, MinigameManager, JournalManager, ObjectiveManager) in the inspector.
+6. Add a `CutscenePlayer` GameObject to the scene, assign any placeholder video or leave the clip empty, and drag that component into the `cutscenePlayer` field of the `RegionFlowController`.
+7. Ensure the guide NPC calls `RegionFlowController.StartRegion(RegionMode.Cebu)` (or Ilocos) after its final dialogue node (see the earlier checklist).
+8. Play the scene – the flow should now continue even without a real cut‑scene video.
+
+If you later obtain the real cinematic, replace the placeholder video clip on the `CutscenePlayer` component; no other code changes are required.
