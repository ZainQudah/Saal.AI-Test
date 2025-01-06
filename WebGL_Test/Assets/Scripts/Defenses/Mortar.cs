using UnityEngine;

/// <summary>
/// Represents a Mortar that targets a location, falls to the target, and destroys threats
/// within a specified radius on impact.
/// </summary>
public class Mortar : Defense
{
    #region Serialized Fields (Unity Inspector)

    [Header("Mortar Settings")] [Tooltip("Descent speed of the mortar (units per second).")] [SerializeField]
    private float speed = 20f;

    #endregion

    #region Private Fields

    private bool hasHit = false; // Ensures threats are only hit once per mortar.
    private Vector3 startPosition; // Holds the starting position of the mortar.

    #endregion

    #region Public Methods

    /// <summary>
    /// Deploys the mortar to a specified target position and initializes its descent.
    /// </summary>
    /// <param name="target">The target position for the mortar to hit.</param>
    public override void Deploy(Vector3 target)
    {
        // Set the initial position of the mortar high above the target location.
        transform.position = new Vector3(target.x, target.y + 60f, target.z);

        // Display the AoE indicator at the target location.
        ShowIndicator(target, radius);
    }

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Continuously moves the mortar downward at a constant speed.
    /// </summary>
    private void Update()
    {
        transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
    }

    /// <summary>
    /// Detects collision with objects in the world and triggers the mortar's explosion effect.
    /// Handles area damage within the defined radius.
    /// </summary>
    /// <param name="other">The collider of the object in contact with the mortar.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Handle impact visual and sound effects.
        Instantiate(particleSystem, transform.position, Quaternion.identity); // Spawn explosion particle system.
        SoundManager.Instance.ExplosionSfx();

        // Perform area damage detection using a sphere.
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hitColliders)
        {
            // Check for threats tagged as "Tank".
            if (hit.CompareTag("Tank") && !hasHit)
            {
                hasHit = true; // Ensure the mortar only affects a threat once.

                // Destroy the tank's parent object (assumes tanks are children of another object).
                Destroy(hit.transform.parent.gameObject);

                // Notify the game manager that a threat has been neutralized.
                GameManager.Instance.ThreatAverted();
                Debug.Log("Threat  Tank neutralized");
            }
        }
        
        hasHit = false;

        // Destroy the mortar after impact.
        ReturnToPool();
        
    }

    /// <summary>
    /// Visualizes the AoE radius of the mortar in the Unity Editor for debugging purposes.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Use red to represent the mortar's radius.
        Gizmos.DrawWireSphere(transform.position, radius); // Draw the radius wireframe.
    }

    #endregion
}