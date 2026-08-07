using System.Collections;
using UnityEngine;

/// <summary>
/// Attach this to any NPC to make them play random idle animations occasionally.
/// </summary>
public class NPCRandomIdle : MonoBehaviour
{
    [Tooltip("The default looping idle animation state name (e.g. 'Breathing_Idle')")]
    public string defaultIdleState = "Breathing_Idle";

    [Tooltip("A list of animation state names to pick from randomly (e.g. 'Stretch', 'LookAround')")]
    public string[] randomIdleStates;

    [Tooltip("Minimum time (in seconds) to wait before playing a random idle")]
    public float minWaitTime = 5f;

    [Tooltip("Maximum time (in seconds) to wait before playing a random idle")]
    public float maxWaitTime = 15f;

    [Tooltip("How long to wait for the random animation to finish before returning to default idle")]
    public float randomAnimDuration = 3f;

    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();

        if (_animator != null && randomIdleStates != null && randomIdleStates.Length > 0)
        {
            StartCoroutine(RandomIdleRoutine());
        }
    }

    private IEnumerator RandomIdleRoutine()
    {
        while (true)
        {
            // 1. Play the default idle
<<<<<<< HEAD
<<<<<<< HEAD
            SafeCrossFade(defaultIdleState, 0.25f);
=======
            _animator.CrossFadeInFixedTime(defaultIdleState, 0.25f);
>>>>>>> 8b2d5a45bf39c6000f4e66ab15743a7dab84d6b7
=======
            _animator.CrossFadeInFixedTime(defaultIdleState, 0.25f);
>>>>>>> upstream/irah-version

            // 2. Wait for a random amount of time
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            // 3. Pick a random animation and play it
            string randomAnim = randomIdleStates[Random.Range(0, randomIdleStates.Length)];
<<<<<<< HEAD
<<<<<<< HEAD
            SafeCrossFade(randomAnim, 0.25f);
=======
            _animator.CrossFadeInFixedTime(randomAnim, 0.25f);
>>>>>>> 8b2d5a45bf39c6000f4e66ab15743a7dab84d6b7
=======
            _animator.CrossFadeInFixedTime(randomAnim, 0.25f);
>>>>>>> upstream/irah-version

            // 4. Wait for the random animation to finish before looping back to default
            yield return new WaitForSeconds(randomAnimDuration);
        }
    }
<<<<<<< HEAD
<<<<<<< HEAD

    private void SafeCrossFade(string stateName, float duration)
    {
        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            if (_animator.HasState(0, Animator.StringToHash(stateName)))
            {
                _animator.CrossFadeInFixedTime(stateName, duration, 0);
            }
        }
    }
}

=======
}
>>>>>>> 8b2d5a45bf39c6000f4e66ab15743a7dab84d6b7
=======
}
>>>>>>> upstream/irah-version
