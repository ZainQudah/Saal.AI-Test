using UnityEngine;

/// <summary>
/// Represents a drone threat that moves toward a target and triggers an explosion upon collision with the player's base.
/// Inherits movement logic from the Threat base class.
/// </summary>
public class Drone : Threat
{
    #region Public Methods

    /// <summary>
    /// Launches the drone toward the target position.
    /// </summary>
    /// <param name="targetPosition">The position the drone should move towards.</param>
    public override void Launch(Vector3 targetPosition)
    {
        // Call the base class's Launch method to initialize target movement.
        base.Launch(targetPosition);
    }

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Updates the drone's position by using the base Threat class's movement logic.
    /// </summary>
    protected override void Update()
    {
        // Invoke the base class Update method for standard movement behavior.
        base.Update();
    }

    #endregion

    #region Collision Handling

    /// <summary>
    /// Detects collision with the player's base and handles the resulting action.
    /// </summary>
    /// <param name="other">The collision object the drone interacts with.</param>
    private void OnCollisionEnter(Collision other)
    {
        // Check if the drone collided with the player's base.
        if (other.gameObject.CompareTag("Player"))
        {
            // Trigger an explosion effect at the collision point.
            Instantiate(particleSystem, transform.position, transform.rotation);

            // Play explosion sound effects.
            SoundManager.Instance.ExplosionSfx();
            SoundManager.Instance.NegativeCue();
            ScreenEffects.Instance.TriggerRedFlash();


            // Destroy the drone game object after the collision.
            Destroy(gameObject);
        }
    }

    #endregion
}