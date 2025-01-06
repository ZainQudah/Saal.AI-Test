using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Represents a submarine that navigates towards a target and fires missiles upon arrival.
/// Derived from the Threat base class.
/// </summary>
public class Submarine : Threat
{
    #region Serialized Fields (Unity Inspector)

    [Header("Submarine Settings")] [Tooltip("The distance at which the submarine fires its missiles.")] [SerializeField]
    private float firingDistance = 5f;

    #endregion

    #region Private Fields

    private NavMeshAgent agent; // Used for navigation (not currently in use but reserved for future implementation).
    private Transform _target; // The target (e.g., Player's base or a specific water-based target).
    private bool hasFired = false; // Tracks whether the submarine has already fired to prevent repeated behavior.

    #endregion

    #region Public Methods

    /// <summary>
    /// Launches the submarine by assigning its target position.
    /// </summary>
    /// <param name="targetPosition">The position of the target to attack.</param>
    public override void Launch(Vector3 targetPosition)
    {
        // Assign the target to a game object with the "WaterTarget" tag.
        _target = GameObject.FindGameObjectWithTag("WaterTarget").transform;
    }

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Updates the submarine's behavior every frame.
    /// Moves it towards the target and triggers actions on arrival.
    /// </summary>
    private new void Update()
    {
        // Ensure the target exists before attempting to move.
        if (_target == null) return;

        // Move toward the target using a normalized direction vector.
        Vector3 direction = (_target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Check if the submarine is close enough to fire and hasn't already fired.
        if (Vector3.Distance(transform.position, _target.position) <= firingDistance && !hasFired)
        {
            FireMissiles();
        }
    }

    #endregion

    #region Submarine Functionality

    /// <summary>
    /// Handles missile firing logic and associated visual/audio effects.
    /// </summary>
    private void FireMissiles()
    {
        hasFired = true; // Set firing status to true to prevent repeated firing.

        // Reference the player's base to target missile impact.
        Transform playerBase = GameObject.FindGameObjectWithTag("Player").transform;

        // Trigger visual effects and sound effects.
        Instantiate(particleSystem, playerBase.position, playerBase.rotation);
        SoundManager.Instance.ExplosionSfx();
        SoundManager.Instance.NegativeCue();
        ScreenEffects.Instance.TriggerRedFlash();


        // Disable the submarine's collider to avoid further interactions.
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Schedule the submarine's removal after firing missiles.
        Invoke(nameof(RemoveSubmarine), 0.2f);
    }

    /// <summary>
    /// Safely removes the submarine from the scene after its actions are complete.
    /// </summary>
    private void RemoveSubmarine()
    {
        Destroy(gameObject);
    }

    #endregion
}