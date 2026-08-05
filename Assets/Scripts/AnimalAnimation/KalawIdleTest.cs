using UnityEngine;

public class KalawIdleTest : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("Idle Timing")]
    public float minIdleStillTime = 4f;
    public float maxIdleStillTime = 8f;

    [Header("Simple Idle Timing")]
    public float minSimpleIdleTime = 2f;
    public float maxSimpleIdleTime = 4f;

    [Header("Pointing Timing")]
    public float minPointingTime = 2f;
    public float maxPointingTime = 4f;

    private float timer;
    private float currentDuration;
    private bool _isActive = true;

    private enum KalawState
    {
        IdleStill,
        SimpleIdle,
        Pointing
    }

    private KalawState currentState = KalawState.IdleStill;

    void Start()
    {
        SetIdleStillTimer();
    }

    void Update()
    {
        if (!_isActive) return;

        timer += Time.deltaTime;

        if (timer >= currentDuration)
        {
            timer = 0f;

            switch (currentState)
            {
                case KalawState.IdleStill:

                    int randomChoice = Random.Range(0, 2);

                    if (randomChoice == 0)
                    {
                        SafeSetTrigger("DoIdleMove");
                        currentState = KalawState.SimpleIdle;
                        SetSimpleIdleTimer();
                    }
                    else
                    {
                        SafeSetTrigger("DoPointing");
                        currentState = KalawState.Pointing;
                        SetPointingTimer();
                    }

                    break;

                case KalawState.SimpleIdle:

                    SafeSetTrigger("ReturnToStill");
                    currentState = KalawState.IdleStill;
                    SetIdleStillTimer();

                    break;

                case KalawState.Pointing:

                    SafeSetTrigger("ReturnFromPointing");
                    currentState = KalawState.IdleStill;
                    SetIdleStillTimer();

                    break;
            }
        }
    }

    // ── WRONG ANSWER REACTION ─────────────────────────────

    /// <summary>
    /// Plays the wrong-answer reaction animation once, waits for it to
    /// finish, then returns Kalaw to idle_still naturally.
    /// Called via OnWrongAnswer UnityEvent wired in the Inspector.
    /// </summary>
    public void PlayWrongAnswerReaction()
    {
        StopCoroutine(nameof(WrongAnswerCoroutine)); // cancel if already running
        StartCoroutine(nameof(WrongAnswerCoroutine));
    }

    private System.Collections.IEnumerator WrongAnswerCoroutine()
    {
        // Signal to DialogueManager that the reaction is in progress
        var npc = GetComponent<InteractableNPC>();
        if (npc != null) npc.isWrongAnswerPlaying = true;

        // Fire the trigger — AnyState → idle (exit time OFF means it interrupts immediately)
        SafeSetTrigger("WrongAnswer");

        // Wait one frame so the animator has time to transition into the idle state
        yield return null;

        // Wait until the idle animation has fully played through (normalizedTime >= 1)
        // Safety timeout of 5 seconds in case something goes wrong
        float timeout = 5f;
        float elapsed = 0f;
        while (elapsed < timeout)
        {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("idle") && info.normalizedTime >= 1f)
                break;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Signal done — DialogueManager will now re-show the question
        if (npc != null) npc.isWrongAnswerPlaying = false;

        Debug.Log("[KalawIdleTest] Wrong-answer reaction finished.");
    }

    // ── Public API ─────────────────────────────────────────────────

    /// <summary>
    /// Stops the idle system and triggers a continuous flying animation.
    /// Use this for the correct answer in your dialogue.
    /// </summary>
    public void StartFlying()
    {
        _isActive = false;
        
        // Reset existing triggers to ensure clean transition
        SafeResetTrigger("DoIdleMove");
        SafeResetTrigger("DoPointing");
        SafeResetTrigger("ReturnToStill");
        SafeResetTrigger("ReturnFromPointing");

        SafeSetTrigger("StartFlying");
        Debug.Log("[KalawIdleTest] Continuous flying started.");
    }

    public void StopIdleSystem()
    {
        _isActive = false;

        SafeResetTrigger("DoIdleMove");
        SafeResetTrigger("DoPointing");

        if (currentState == KalawState.SimpleIdle)
            SafeSetTrigger("ReturnToStill");
        else if (currentState == KalawState.Pointing)
            SafeSetTrigger("ReturnFromPointing");

        currentState = KalawState.IdleStill;
        timer = 0f;

        Debug.Log("[KalawIdleTest] Idle system stopped.");
    }

    public void ResumeIdleSystem()
    {
        _isActive = true;
        timer = 0f;
        currentState = KalawState.IdleStill;
        SetIdleStillTimer();

        Debug.Log("[KalawIdleTest] Idle system resumed.");
    }

    // ── Timers ─────────────────────────────────────────────────────

    void SetIdleStillTimer()
    {
        currentDuration = Random.Range(minIdleStillTime, maxIdleStillTime);
    }

    void SetSimpleIdleTimer()
    {
        currentDuration = Random.Range(minSimpleIdleTime, maxSimpleIdleTime);
    }

    void SetPointingTimer()
    {
        currentDuration = Random.Range(minPointingTime, maxPointingTime);
    }

    private void SafeSetTrigger(string triggerName)
    {
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            foreach (var param in animator.parameters)
            {
                if (param.name == triggerName)
                {
                    SafeSetTrigger(triggerName);
                    return;
                }
            }
        }
    }

    private void SafeResetTrigger(string triggerName)
    {
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            foreach (var param in animator.parameters)
            {
                if (param.name == triggerName)
                {
                    SafeResetTrigger(triggerName);
                    return;
                }
            }
        }
    }
}
