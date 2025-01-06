using UnityEngine;

/// <summary>
/// Represents a water mine that destroys submarines on impact
/// and self-destructs after a specified duration if no threats are detected.
/// </summary>
public class WaterMine : Defense
{
    #region Serialized Fields (Unity Inspector)

    [Header("Water Mine Settings")] [Tooltip("Duration in seconds before the mine self-destructs.")] [SerializeField]
    private float activeDuration = 10f;

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Initializes the water mine and schedules its self-destruction after the active duration.
    /// </summary>
    private void Start()
    {
        Invoke(nameof(SelfDestruct), activeDuration); // Schedule self-destruction after activeDuration.
    }

    /// <summary>
    /// Detects collisions with submarines and triggers destruction effects.
    /// </summary>
    /// <param name="other">The collider of the object that triggered the mine.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is a submarine.
        if (other.CompareTag("Submarine"))
        {
            // Destroy the submarine and trigger explosion effects.
            Destroy(other.gameObject); // Remove the detected submarine.
            Instantiate(particleSystem, transform.position, transform.rotation); // Create explosion effect.
            SoundManager.Instance.ExplosionSfx(); // Play explosion sound effect.

            // Notify the game manager that the threat was neutralized.
            GameManager.Instance.ThreatAverted();

            // Destroy the water mine after impact.
            ReturnToPool();
        }
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Handles the self-destruction of the water mine after its active duration expires.
    /// </summary>
    private void SelfDestruct()
    {
       ReturnToPool(); // Remove the water mine object.
    }

    #endregion
}