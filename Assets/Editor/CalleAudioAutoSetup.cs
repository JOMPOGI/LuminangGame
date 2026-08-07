using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CalleAudioAutoSetup : EditorWindow
{
    [MenuItem("Tools/Setup CALLE_CRISOLOGO Audio")]
    public static void SetupAudio()
    {
        // Load audio clips
        AudioClip catClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/SFX/cat_meow.mp3");
        AudioClip carriageClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/SFX/Horse_Carriage.mp3");
        AudioClip eatingClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/SFX/Horse_Eating.mp3");
        AudioClip neighClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/SFX/Horse_Neigh.mp3");

        bool missingFiles = false;
        if (catClip == null) { Debug.LogError("Missing: Assets/SFX/cat_meow.mp3"); missingFiles = true; }
        if (carriageClip == null) { Debug.LogError("Missing: Assets/SFX/Horse_Carriage.mp3"); missingFiles = true; }
        if (eatingClip == null) { Debug.LogError("Missing: Assets/SFX/Horse_Eating.mp3"); missingFiles = true; }
        if (neighClip == null) { Debug.LogError("Missing: Assets/SFX/Horse_Neigh.mp3"); missingFiles = true; }
        
        if (missingFiles)
        {
            Debug.LogError("Cannot setup audio because some audio files are missing. Please check the paths.");
            return;
        }

        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        int setupCount = 0;

        // Generate or Update Cat Dialogue Node
        DialogueNode catDialogue = AssetDatabase.LoadAssetAtPath<DialogueNode>("Assets/Dialogues/CatMeowDialogue.asset");
        if (catDialogue == null)
        {
            catDialogue = ScriptableObject.CreateInstance<DialogueNode>();
            if (!AssetDatabase.IsValidFolder("Assets/Dialogues")) AssetDatabase.CreateFolder("Assets", "Dialogues");
            AssetDatabase.CreateAsset(catDialogue, "Assets/Dialogues/CatMeowDialogue.asset");
        }
        
        // Always apply these settings in case they changed
        catDialogue.speakerName = "Cat";
        catDialogue.dialogueText = "*Meow!*";
        EditorUtility.SetDirty(catDialogue);
        AssetDatabase.SaveAssets();

        foreach (GameObject go in allObjects)
        {
            string lowerName = go.name.ToLower();
            
            // If this is a fruit/clue, attach QuestTargetMarker for the path tracker
            if (lowerName.Contains("fruit") || lowerName.Contains("prutas") || lowerName.Contains("clue"))
            {
                QuestTargetMarker marker = go.GetComponent<QuestTargetMarker>();
                if (marker == null) marker = go.AddComponent<QuestTargetMarker>();
                marker.requiredObjective = "Look for clues";
                EditorUtility.SetDirty(go);
            }
            
            // Check if this object is part of a carriage (has a parent with carriage in name)
            bool isPartofCarriage = false;
            Transform curr = go.transform.parent;
            while (curr != null)
            {
                string pName = curr.name.ToLower();
                if (pName.Contains("carriage") || pName.Contains("kalesa") || pName.Contains("cart"))
                {
                    isPartofCarriage = true;
                    break;
                }
                curr = curr.parent;
            }

            // 1. Cat (Interactable)
            if (lowerName.Contains("cat") || lowerName.Contains("kuting"))
            {
                // Remove ProximityAudio if it exists
                ProximityAudio existingAudio = go.GetComponent<ProximityAudio>();
                if (existingAudio != null) DestroyImmediate(existingAudio);

                // Add Interactable setup
                InteractableNPC npc = go.GetComponent<InteractableNPC>();
                if (npc == null) npc = go.AddComponent<InteractableNPC>();
                npc.defaultDialogue = catDialogue;
                npc.promptText = "Pet Cat";

                PlayAudioOnInteract pa = go.GetComponent<PlayAudioOnInteract>();
                if (pa == null) pa = go.AddComponent<PlayAudioOnInteract>();

                AudioSource source = go.GetComponent<AudioSource>();
                if (source == null) source = go.AddComponent<AudioSource>();
                source.clip = catClip;
                source.spatialBlend = 1f;
                source.playOnAwake = false;

                EditorUtility.SetDirty(go);
                setupCount++;
            }
            // 2. Horse Carriage
            else if (lowerName.Contains("carriage") || lowerName.Contains("kalesa") || lowerName.Contains("cart"))
            {
                // Carriage volume is 1.0f (Louder)
                SetupProximityAudio(go, carriageClip, true, false, 0f, 25f, 1.0f);
                setupCount++;
            }
            // 3. Eating Horse
            else if (lowerName.Contains("eating") && lowerName.Contains("horse"))
            {
                SetupProximityAudio(go, eatingClip, true, false, 0f, 15f, 0.6f);
                setupCount++;
            }
            // 4. Other Horses (including Carriage Horses)
            else if (lowerName.Contains("horse"))
            {
                if (isPartofCarriage)
                {
                    // Horse in carriage neighs ONCE
                    SetupProximityAudio(go, neighClip, false, true, 5.0f, 15f, 0.6f);
                }
                else
                {
                    // Regular horses neigh periodically
                    SetupProximityAudio(go, neighClip, false, false, 5.0f, 15f, 0.6f);
                }
                setupCount++;
            }
        }

        Debug.Log($"[CalleAudioAutoSetup] Completed! Applied audio setup to {setupCount} objects in the scene.");
        EditorUtility.DisplayDialog("Audio Setup Complete", $"Successfully applied ProximityAudio to {setupCount} objects.\n\nPlease check the Unity Console for details.", "OK");
    }

    private static void SetupProximityAudio(GameObject go, AudioClip clip, bool isContinuous, bool playOnce, float interval, float triggerDist, float volume)
    {
        // Prevent duplicate setups
        ProximityAudio existing = go.GetComponent<ProximityAudio>();
        if (existing != null)
        {
            AudioSource existingSource = go.GetComponent<AudioSource>();
            if (existingSource != null && existingSource.clip == clip)
            {
                // Force update volume just in case it was run before with different volume
                existingSource.volume = volume;
                existing.triggerDistance = triggerDist;
                existing.playOnce = playOnce;
                return; // Already configured properly
            }
        }

        // Add the proximity audio component
        ProximityAudio pa = go.GetComponent<ProximityAudio>();
        if (pa == null) pa = go.AddComponent<ProximityAudio>();
        
        pa.isContinuous = isContinuous;
        pa.playOnce = playOnce;
        pa.playInterval = interval;
        pa.triggerDistance = triggerDist;

        // The ProximityAudio script automatically requires AudioSource and SFXVolumeSync
        AudioSource source = go.GetComponent<AudioSource>();
        if (source != null)
        {
            source.clip = clip;
            source.volume = volume;
        }
        
        Debug.Log($"Applied {clip.name} setup to {go.name}");
        EditorUtility.SetDirty(go);
    }
}
