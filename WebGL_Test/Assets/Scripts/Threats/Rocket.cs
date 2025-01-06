using UnityEngine;

/// <summary>
/// Represents a rocket that travels along a parabolic arc toward a target.
/// </summary>
public class Rocket : Threat
{
    #region Serialized Fields (Unity Inspector)

    [Header("Rocket Settings")]
    [Tooltip("The maximum height of the parabolic arc the rocket follows.")]
    [SerializeField]
    private float arcHeight = 10f;

    [Tooltip("Particle system prefab for visual effect triggered when intercepted by a shield.")] [SerializeField]
    private GameObject particleSystem2;

    #endregion

    #region Private Fields

    // The starting position of the rocket.
    private Vector3 startPosition;

    // The target position the rocket moves towards.
    private Vector3 targetPosition;

    // Tracks the rocket's progress along the arc, between 0 (start) and 1 (target).
    private float progress = 0f;

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the rocket's starting and target positions.
    /// </summary>
    /// <param name="target">The target position the rocket should hit.</param>
    public override void Launch(Vector3 target)
    {
        startPosition = transform.position; // Set the start position to the rocket's current position.
        targetPosition = target; // Set the target position.
    }

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Updates the rocket's position and rotation as it moves along the parabolic arc.
    /// </summary>
    protected override void Update()
    {
        // If the rocket hasn't reached the target, continue moving.
        if (progress < 1f)
        {
            // Increment the progress along the arc based on speed and distance.
            progress += speed * Time.deltaTime / Vector3.Distance(startPosition, targetPosition);

            // Calculate the rocket's current and next positions.
            Vector3 currentPosition = transform.position;
            Vector3 nextPosition = ParabolicArc(startPosition, targetPosition, arcHeight, progress);

            // Update the rocket's position to the next calculated position.
            transform.position = nextPosition;

            // Rotate the rocket to face the direction of movement.
            Vector3 direction = (nextPosition - currentPosition).normalized;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    /// <summary>
    /// Handles collisions with other objects, such as shields or the player.
    /// </summary>
    /// <param name="other">The collider of the object the rocket collided with.</param>
    private void OnTriggerEnter(Collider other)
    {
        // If the rocket is intercepted by an anti-rocket shield, trigger interception logic.
        if (other.CompareTag("Shield"))
        {
            // Instantiate the particle effect and destroy the rocket.
            Instantiate(particleSystem2, transform.position, transform.rotation);
            Destroy(gameObject);

            // Notify the game manager that the threat was averted.
            GameManager.Instance.ThreatAverted();
            SoundManager.Instance.PlaySoundOneShot(SoundManager.Instance.empSound);

        }

        // If the rocket hits the player's base, trigger explosion logic.
        else if (other.CompareTag("Player"))
        {
            // Instantiate the explosion particle effect and play sounds.
            Instantiate(particleSystem, transform.position, transform.rotation);
            SoundManager.Instance.ExplosionSfx();
            SoundManager.Instance.NegativeCue();
            ScreenEffects.Instance.TriggerRedFlash();


            // Destroy the rocket.
            Destroy(gameObject);
        }
    }

    #endregion

    #region Rocket Functionality

    /// <summary>
    /// Calculates the rocket's position along a parabolic arc based on a given progress value.
    /// </summary>
    /// <param name="start">The starting position of the arc.</param>
    /// <param name="end">The end (target) position of the arc.</param>
    /// <param name="height">The maximum height of the arc.</param>
    /// <param name="t">The progress value, ranging from 0 (start) to 1 (end).</param>
    /// <returns>The calculated position along the parabolic arc.</returns>
    private Vector3 ParabolicArc(Vector3 start, Vector3 end, float height, float t)
    {
        // Interpolate horizontally between the start and end positions.
        Vector3 horizontal = Vector3.Lerp(start, end, t);

        // Add a vertical parabolic component based on the progress.
        float vertical = Mathf.Sin(t * Mathf.PI) * height;

        // Combine the horizontal and vertical components to get the final position.
        return new Vector3(horizontal.x, horizontal.y + vertical, horizontal.z);
    }

    #endregion
}