using UnityEngine;

public class QuestIdleManager : MonoBehaviour
{
    [Header("Settings")]
    public string requiredObjective;
    public string stopObjective;
    public string animationTrigger = "Waving";
    public string stopAnimationTrigger = "StopWaving";
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
            _manuallyStopped = false;
            _lastObjective = current;
        }

        // Check if we should explicitly stop
        if (!string.IsNullOrEmpty(stopObjective) && !string.IsNullOrEmpty(current) && 
            current.StartsWith(stopObjective, System.StringComparison.OrdinalIgnoreCase))
        {
            if (_isWaving) StopQuestIdle();
            _manuallyStopped = true;
            return;
        }

        bool objectiveActive = !string.IsNullOrEmpty(current) && current.StartsWith(requiredObjective, System.StringComparison.OrdinalIgnoreCase);

        if (objectiveActive && !_isWaving && !_manuallyStopped)
        {
            StartQuestIdle();
        }
        else if (!objectiveActive && _isWaving)
        {
            StopQuestIdle();
        }
    }

    public void StartQuestIdle()
    {
        if (targetAnimator == null) return;
        _isWaving = true;
        targetAnimator.SetTrigger(animationTrigger);
        Debug.Log($"[{gameObject.name}] Starting Quest Idle: {animationTrigger}");
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
}
