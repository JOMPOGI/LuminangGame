using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the eating horse animation loop:
/// Eating clip -> random Idle pause -> repeat
/// 
/// Setup:
/// 1. Attach this script to the eating horse GameObject (or its parent).
/// 2. Make sure the Animator uses the EatingHorse controller.
/// 3. The controller needs two states named exactly: "Eating" and "Idle"
///    with NO automatic transitions (this script drives them manually).
/// </summary>
public class HorseEating : MonoBehaviour
{
    [Header("Animation State Names")]
    [Tooltip("Must match the state name in the EatingHorse Animator Controller")]
    public string eatingStateName = "Eating";

    [Tooltip("Must match the state name in the EatingHorse Animator Controller")]
    public string idleStateName = "Idle";

    [Header("Timing")]
    [Tooltip("How long the Eating animation plays before switching to Idle (seconds). " +
             "Your Eating clip is 180 frames at 30fps = 6 seconds.")]
    public float eatDuration = 6f;

    [Tooltip("Minimum pause (in seconds) the horse stays idle between eating cycles.")]
    public float minIdlePause = 2f;

    [Tooltip("Maximum pause (in seconds) the horse stays idle between eating cycles.")]
    public float maxIdlePause = 6f;

    private Animator _animator;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();

        if (_animator == null)
        {
            Debug.LogError($"[HorseEating] No Animator found on {gameObject.name} or its children!");
            return;
        }

        StartCoroutine(EatLoop());
    }

    private IEnumerator EatLoop()
    {
        while (true)
        {
            // --- Play Eating ---
            _animator.Play(eatingStateName);
            yield return new WaitForSeconds(eatDuration);

            // --- Switch to Idle with a random pause ---
            _animator.Play(idleStateName);
            float idleTime = Random.Range(minIdlePause, maxIdlePause);
            yield return new WaitForSeconds(idleTime);
        }
    }
}
