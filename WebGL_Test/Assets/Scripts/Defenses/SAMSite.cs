using UnityEngine;

/// <summary>
/// Represents a Surface-to-Air Missile (SAM) site that detects and destroys airborne threats (Airstrike objects) within a specified radius.
/// It self-destructs after a defined active duration.
/// </summary>
public class SAMSite : Defense
{
    #region Serialized Fields (Unity Inspector)

    [Header("SAM Weapon Settings")]
    [Tooltip("Transform of the weapon (child object) used for aiming at targets.")]
    [SerializeField]
    private Transform weapon;

    [Header("Detection Settings")] [Tooltip("Radius within which the SAM Site detects threats.")] [SerializeField]
    private float detectionRadius = 15f;

    [Header("Timing Settings")] [Tooltip("Time spent aiming at the target before firing.")] [SerializeField]
    private float aimDuration = 2f;

    [Tooltip("Time before the SAM Site self-destructs after being deployed.")] [SerializeField]
    private float activeDuration = 10f;

    [Header("Effect Settings")] [Tooltip("Particle system to display when the weapon is fired.")] [SerializeField]
    private GameObject particleSystem2;

    [Tooltip("Position from where the weapon launches projectiles or visual effects.")] [SerializeField]
    private Transform firingPoint;

    #endregion

    #region Private Fields

    private Airstrike targetThreat; // Current threat being targeted by the SAM Site.
    private bool isEngaging = false; // Prevents multiple engagements simultaneously.

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Initializes the SAM Site and sets a timer for self-destruction after it has been active for the defined duration.
    /// </summary>
    private void Start()
    {
        Invoke(nameof(DestroySAMSite), activeDuration); // Schedule self-destruction.
    }

    /// <summary>
    /// Continuously detects threats and engages targets if available.
    /// </summary>
    private void Update()
    {
        if (!isEngaging)
        {
            FindTarget(); // Look for threats within detection range.
        }

        if (targetThreat != null)
        {
            AimAtTarget(); // Aim the weapon at the currently detected target.
        }
    }

    #endregion

    #region Targeting and Engagement

    /// <summary>
    /// Finds a suitable target (Airstrike threat) within the detection radius.
    /// Engages the first valid target found.
    /// </summary>
    private void FindTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var hitCollider in hitColliders)
        {
            Airstrike threat = hitCollider.GetComponent<Airstrike>();
            if (threat != null) // Only target airborne threats (Airstrike).
            {
                targetThreat = threat;
                isEngaging = true; // Prevent simultaneous targeting of multiple threats.
                StartCoroutine(EngageTarget());
                break; // Stop looking once a target is found.
            }
        }
    }

    /// <summary>
    /// Smoothly rotates the weapon to aim at the current target.
    /// </summary>
    private void AimAtTarget()
    {
        if (targetThreat != null)
        {
            // Calculate direction to the target.
            Vector3 direction = (targetThreat.transform.position - weapon.position).normalized;

            // Smoothly rotate the weapon towards the target.
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            weapon.rotation = Quaternion.Slerp(weapon.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    /// <summary>
    /// Engages the current target by firing the weapon after the aiming duration.
    /// Displays effects and destroys the target.
    /// </summary>
    /// <returns>A coroutine that waits for the aiming duration before firing.</returns>
    private System.Collections.IEnumerator EngageTarget()
    {
        yield return new WaitForSeconds(aimDuration); // Wait for the weapon to aim properly.

        if (targetThreat != null && !targetThreat.bombDropped) // Ensure that a valid target is still available.
        {
            // Create firing effects and sound effects.
            Instantiate(particleSystem2, firingPoint.position, firingPoint.rotation);
            Instantiate(particleSystem, targetThreat.transform.position, targetThreat.transform.rotation);
            SoundManager.Instance.ExplosionSfx();

            // Destroy the targeted threat and notify the game manager.
            Destroy(targetThreat.gameObject);
            GameManager.Instance.ThreatAverted();
        }

        // Reset the engagement flags after firing.
        isEngaging = false;
        targetThreat = null;
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Destroys the SAM Site object after its active duration has expired.
    /// </summary>
    private void DestroySAMSite()
    {
         ReturnToPool();    
    }

    /// <summary>
    /// Visualizes the SAM Site's detection radius in the Unity Editor for debugging purposes.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Draw the detection radius using a red wireframe.
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    #endregion
}