using System.Collections;
using UnityEngine;

/// <summary>
/// Moves a horse along a list of waypoints in a loop (circular patrol).
/// Optionally pauses at each waypoint to play an Idle animation.
///
/// Setup:
/// 1. Create an empty GameObject in the scene called "HorseWaypoints".
/// 2. Add child empty GameObjects inside it for each waypoint (e.g. WP_01, WP_02, ...).
///    Arrange them in a rough circle or path around the scene.
/// 3. Attach this script to the walking horse.
/// 4. Drag the waypoint child objects into the Waypoints list in the Inspector.
/// 5. Remove or disable the old HorseMove script component.
///
/// For the idle-patrol variant:
/// - Enable "Pause At Waypoints"
/// - Make sure the Animator Controller has both a "Walk" and "Idle" state.
/// </summary>
public class HorsePatrol : MonoBehaviour
{
    [Header("Waypoints")]
    [Tooltip("Assign the waypoint Transforms in order around the patrol path.")]
    public Transform[] waypoints;

    [Header("Movement")]
    [Tooltip("Walking speed in units per second.")]
    public float speed = 1.5f;

    [Tooltip("How fast the horse rotates to face the next waypoint.")]
    public float rotateSpeed = 3f;

    [Tooltip("How close the horse must get to a waypoint before moving to the next one.")]
    public float waypointReachDistance = 0.8f;

    [Header("Idle Pauses (optional)")]
    [Tooltip("If enabled, the horse will stop and idle at each waypoint before continuing.")]
    public bool pauseAtWaypoints = false;

    [Tooltip("Minimum seconds the horse idles at a waypoint.")]
    public float minIdlePause = 2f;

    [Tooltip("Maximum seconds the horse idles at a waypoint.")]
    public float maxIdlePause = 5f;

    [Tooltip("Name of the Walk state in the Animator Controller.")]
    public string walkStateName = "Walk";

    [Tooltip("Name of the Idle state in the Animator Controller.")]
    public string idleStateName = "Idle";

    // ── Internals ──────────────────────────────────────────────
    private int      _currentWaypointIndex = 0;
    private bool     _isIdling             = false;
    private Animator _animator;

    void Start()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError($"[HorsePatrol] No waypoints assigned on {gameObject.name}!");
            enabled = false;
            return;
        }

        // Grab animator from self or any child (e.g. AnimalArmature)
        _animator = GetComponentInChildren<Animator>();

        if (pauseAtWaypoints && _animator == null)
            Debug.LogWarning($"[HorsePatrol] 'Pause At Waypoints' is on but no Animator found on {gameObject.name}!");

        // Snap to the first waypoint's Y level (keeps horse on the ground)
        Vector3 startPos = transform.position;
        startPos.y = waypoints[0].position.y;
        transform.position = startPos;

        // Start in walk state
        SetWalking();
    }

    void Update()
    {
        // While idling, don't move
        if (_isIdling || waypoints.Length == 0) return;

        Transform target = waypoints[_currentWaypointIndex];

        // Ignore Y difference so the horse doesn't tilt up/down
        Vector3 directionToTarget = target.position - transform.position;
        directionToTarget.y = 0f;

        // Check if we've reached the current waypoint
        if (directionToTarget.magnitude <= waypointReachDistance)
        {
            if (pauseAtWaypoints)
            {
                StartCoroutine(IdleAtWaypoint());
            }
            else
            {
                _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
            }
            return;
        }

        // Smoothly rotate toward the target direction
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotateSpeed * Time.deltaTime
        );

        // Move forward along the horse's own forward axis
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private IEnumerator IdleAtWaypoint()
    {
        _isIdling = true;
        SetIdling();

        float waitTime = Random.Range(minIdlePause, maxIdlePause);
        yield return new WaitForSeconds(waitTime);

        // Advance to next waypoint then resume walking
        _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
        SetWalking();
        _isIdling = false;
    }

    private void SetWalking()
    {
        if (_animator != null)
            _animator.Play(walkStateName);
    }

    private void SetIdling()
    {
        if (_animator != null)
            _animator.Play(idleStateName);
    }

#if UNITY_EDITOR
    // Draws the patrol path in the Scene view for easy debugging
    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            // Draw sphere at each waypoint
            Gizmos.DrawSphere(waypoints[i].position, 0.3f);

            // Draw line to next waypoint (wraps around to first)
            Transform next = waypoints[(i + 1) % waypoints.Length];
            if (next != null)
                Gizmos.DrawLine(waypoints[i].position, next.position);
        }
    }
#endif
}
