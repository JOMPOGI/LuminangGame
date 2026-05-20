using UnityEngine;

public class QuestIdleManager : MonoBehaviour
{
    [Header("Settings")]
    public string requiredObjective;
    public string stopObjective;
    public string animationTrigger = "Waving";
    public string stopAnimationTrigger = "StopWaving";
    public bool loopAnimation = true;
    public Animator targetAnimator;
    
    private bool _isWaving = false;
    private bool _manuallyStopped = false;
    private string _lastObjective;

    void Update()
    {
        if (ObjectiveManager.Instance == null || targetAnimator == null) return;

        string current = ObjectiveManager.Instance.CurrentObjective;
        
        // If the objective changed, reset our manual stop memory
        if (current != _lastObjective)
        {
            Debug.Log($"[{gameObject.name}] Objective changed from '{_lastObjective}' to '{current}'. Resetting manual stop.");
            _manuallyStopped = false;
            _lastObjective = current;
        }

        // Check if we should explicitly stop
        if (!string.IsNullOrEmpty(stopObjective) && !string.IsNullOrEmpty(current) && 
            current.StartsWith(stopObjective, System.StringComparison.OrdinalIgnoreCase))
        {
            if (_isWaving) 
            {
                Debug.Log($"[{gameObject.name}] Stop objective '{stopObjective}' detected. Stopping animation.");
                StopQuestIdle();
            }
            _manuallyStopped = true;
            return;
        }

        bool objectiveActive = !string.IsNullOrEmpty(current) && current.StartsWith(requiredObjective, System.StringComparison.OrdinalIgnoreCase);

        if (objectiveActive && !_isWaving && !_manuallyStopped)
        {
            Debug.Log($"[{gameObject.name}] Conditions met! Starting Quest Idle. Objective: {current}");
            StartQuestIdle();
        }
        else if (!objectiveActive && _isWaving)
        {
            Debug.Log($"[{gameObject.name}] Objective no longer active. Stopping Quest Idle.");
            StopQuestIdle();
        }
    }

    public void StartQuestIdle()
    {
        if (targetAnimator == null || string.IsNullOrEmpty(animationTrigger)) return;
        
        Debug.Log($"[{gameObject.name}] Brute Force Playing State: {animationTrigger}");
        
        // Tell the animator if we want to loop or not
        targetAnimator.SetBool("LoopQuestIdle", loopAnimation);
        
        // This jumps directly to the state name, bypassing triggers and arrows
        targetAnimator.Play(animationTrigger);
        _isWaving = true;
    }

    public void StopQuestIdle()
    {
        if (!_isWaving || targetAnimator == null) return;
        _isWaving = false;
        _manuallyStopped = true; 
        
        Debug.Log($"[{gameObject.name}] Bulletproof Stop initiated...");

        // 1. Fire the trigger
        if (!string.IsNullOrEmpty(stopAnimationTrigger))
        {
            targetAnimator.SetTrigger(stopAnimationTrigger);
        }

        // 2. Force the state change (using Layer 0)
        targetAnimator.CrossFadeInFixedTime("Breathing_Idle", 0.1f, 0);
        
        // 3. Last resort: Direct Play
        targetAnimator.Play("Breathing_Idle");
    }

    public void StopAnimation()
    {
        if (targetAnimator != null && !string.IsNullOrEmpty(stopAnimationTrigger))
        {
            targetAnimator.SetTrigger(stopAnimationTrigger);
        }
        _isWaving = false;
        _manuallyStopped = true;
        Debug.Log($"[{gameObject.name}] Quest Idle Stopped Manually.");
    }
}
