using UnityEngine;

/// <summary>
/// Represents an anti-rocket shield that protects an area for a limited duration by blocking incoming threats.
/// </summary>
public class AntiRocketShield : Defense
{
    #region Serialized Fields (Unity Inspector)

    [Header("Shield Settings")] [Tooltip("Duration (in seconds) for which the shield remains active.")] [SerializeField]
    private float shieldDuration = 5f;

    #endregion

    #region Private Fields

    private Collider shieldCollider; // Reference to the shield's collider component.

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Initializes the shield by activating it and scheduling its deactivation after the specified duration.
    /// </summary>
    private void Start()
    {
        // Cache the shield's collider component.
        shieldCollider = GetComponent<Collider>();

        // Activate the shield immediately upon creation.
        ActivateShield();

        // Schedule the shield's deactivation after its duration ends.
        Invoke(nameof(DeactivateShield), shieldDuration);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Positions the shield at the specified location.
    /// </summary>
    /// <param name="position">The world position where the shield will be deployed.</param>
    public override void Deploy(Vector3 position)
    {
        // Position the shield at the target location.
        transform.position = position;
    }

    #endregion

    #region Shield Functionality

    /// <summary>
    /// Activates the shield by enabling its collider.
    /// </summary>
    private void ActivateShield()
    {
        if (shieldCollider != null)
        {
            shieldCollider.enabled = true; // Enable the collider to start blocking threats.
        }
    }

    /// <summary>
    /// Deactivates the shield, disables its collider, and destroys the object.
    /// </summary>
    private void DeactivateShield()
    {
       

        // Destroy the shield object after deactivation.
        ReturnToPool();
    }

    #endregion
}