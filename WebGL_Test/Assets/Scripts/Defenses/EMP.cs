using UnityEngine;

/// <summary>
/// Represents an EMP device that falls to a target location,
/// activates upon impact, and destroys threats within a specified radius.
/// </summary>
public class EMP : Defense
{
    #region Serialized Fields (Unity Inspector)

    [Header("EMP Settings")] 
    [Tooltip("Height from which the EMP starts falling towards the target position.")] [SerializeField]
    private float dropHeight = 20f;

    [Tooltip("Rigidbody component controlling physics behavior of the EMP.")] [SerializeField]
    private Rigidbody rb;

    [Header("EMP Fall and Explosion Settings")] [Tooltip("Fall speed of the EMP device.")] [SerializeField]
    private float speed = 10f;

    #endregion

    #region Private Fields

    private bool hasHitGround = false; // Tracks whether the EMP has hit the ground.

    #endregion

    #region Public Methods

    /// <summary>
    /// Deploys the EMP to the target position, initializing its position above the ground
    /// and enabling its area-of-effect (AoE) indicator.
    /// </summary>
    /// <param name="position">The target deployment position.</param>
    public override void Deploy(Vector3 position)
    {
        // Set the initial position high above the target point.
        transform.position = position + Vector3.up * dropHeight;

        // Enable physics to allow the EMP to fall.
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // Display the area of effect (AoE) indicator slightly above the ground.
        Vector3 displayPosition = position + Vector3.up * 1.5f;
        ShowIndicator(displayPosition, radius);
    }

    

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Controls the manual descent of the EMP in case physics simulation is not configured.
    /// </summary>
    private void Update()
    {
        // If the Rigidbody `rb` is not used (optional fall logic).
        transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
    }

    /// <summary>
    /// Reacts to collisions between the EMP and other objects, triggering activation upon impact with "Ground."
    /// </summary>
    /// <param name="collision">The collision object data.</param>
    private void OnCollisionEnter(Collision collision)
    {
        // Prevent multiple triggers once the EMP has hit the ground.
        if (hasHitGround) return;

        // Confirm collision with a ground object.
        if (collision.gameObject.CompareTag("Ground"))
        {
            hasHitGround = true;

            // Stop physics simulation.
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            // Trigger EMP functionality after impact.
            ActivateEMP();
        }
    }

    #endregion

    #region EMP Functionality

    /// <summary>
    /// Activates the EMP, destroying threats within its radius and triggering visual and sound effects.
    /// </summary>
    private void ActivateEMP()
    {
        // Play EMP sound effect.
        SoundManager.Instance.PlaySoundOneShot(SoundManager.Instance.empSound);

        // Detect objects within the radius using a sphere overlap.
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        // Display EMP particle effect at the activation position.
        Instantiate(particleSystem, transform.position, Quaternion.identity);

        // Handle all detected threats within the radius.
        foreach (var hitCollider in hitColliders)
        {
            Drone threat = hitCollider.GetComponent<Drone>();
            if (threat != null)
            {
                // Destroy the threat and notify the Game Manager.
                Destroy(threat.gameObject);
                GameManager.Instance.ThreatAverted();
            }
        }

        // Hide the AoE indicator and destroy the EMP object.
        HideIndicator();
        ReturnToPool();
        hasHitGround = false;

    }

    #endregion
}