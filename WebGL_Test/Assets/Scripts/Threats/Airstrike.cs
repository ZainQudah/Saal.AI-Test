using UnityEngine;

/// <summary>
/// Represents an airstrike plane that flies over a target, drops a bomb, and continues flying.
/// </summary>
public class Airstrike : Threat
{
    #region Serialized Fields (Unity Inspector)

    [Header("Airstrike Settings")] [Tooltip("Prefab of the bomb to be dropped.")] [SerializeField]
    private GameObject bombPrefab;

    [Tooltip("Distance threshold along the x-z plane to determine when to drop the bomb.")] [SerializeField]
    private float dropHeightThreshold = 1f;

    [Tooltip("Height at which the plane flies over the target.")] [SerializeField]
    private float flyOverHeight = 20f;

    [Tooltip("Duration the plane continues flying after dropping the bomb (in seconds).")] [SerializeField]
    private float flightDurationAfterDrop = 5f;

    [Tooltip("Transform representing the bomb drop point.")] [SerializeField]
    private Transform dropPoint;

    #endregion

    #region Private Fields

    public bool bombDropped = false; // Tracks whether the bomb has already been dropped.
    private Collider planeCollider; // Reference to the plane's collider.
    private Vector3 forwardDirection; // Direction to continue flying after the bomb is dropped.

    #endregion

    #region Unity Methods

    /// <summary>
    /// Initializes the airstrike plane's collider and calculates the forward flight direction.
    /// </summary>
    private void Start()
    {
        planeCollider = GetComponent<Collider>();

        // Calculate the forward direction toward the target (ignoring the vertical component).
        forwardDirection = (target - transform.position).normalized;
        forwardDirection.y = 0;
    }

    /// <summary>
    /// Manages the airstrike's behavior, including flying above the target, dropping the bomb, and continuing its flight.
    /// </summary>
    protected override void Update()
    {
        if (!bombDropped)
        {
            FlyAboveTarget();
        }
        else
        {
            ContinueFlying();
        }

        // Drop the bomb when the plane is directly over the target in the x-z plane.
        if (!bombDropped && Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                new Vector3(target.x, 0, target.z)) < dropHeightThreshold)
        {
            DropBomb();
        }
    }

    #endregion

    #region Airstrike Functionality

    /// <summary>
    /// Moves the plane toward the target position (x-z plane) while maintaining a constant height.
    /// </summary>
    private void FlyAboveTarget()
    {
        // Calculate the target position with a fixed flyover height.
        Vector3 targetPosition = new Vector3(target.x, flyOverHeight, target.z);

        // Move the plane towards the adjusted target position.
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Smoothly rotate the plane to face the target position.
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
    }

    /// <summary>
    /// Drops the bomb on the target and handles post-drop events such as visual effects and collider disabling.
    /// </summary>
    private void DropBomb()
    {
        bombDropped = true;

        // Instantiate the bomb at the drop point's position.
        Instantiate(bombPrefab, dropPoint.position, Quaternion.identity);

        // Play visual effects and sounds at the target position.
        Instantiate(particleSystem, target, Quaternion.identity);
        SoundManager.Instance.ExplosionSfx();
        SoundManager.Instance.NegativeCue();
        ScreenEffects.Instance.TriggerRedFlash();

        // Disable the plane's collider to make it untargetable by other systems.
        if (planeCollider != null)
        {
            planeCollider.enabled = false;
        }

        // Schedule the destruction of the plane after it flies for the specified duration.
        Invoke(nameof(DestroyAirstrike), flightDurationAfterDrop);
    }

    /// <summary>
    /// Continues flying in a straight line away from the target after dropping the bomb.
    /// </summary>
    private void ContinueFlying()
    {
        // Move the plane forward in the previously calculated direction.
        transform.position += forwardDirection * speed * Time.deltaTime;

        // Maintain the plane's flying height.
        transform.position = new Vector3(transform.position.x, flyOverHeight, transform.position.z);
    }

    /// <summary>
    /// Destroys the airstrike plane after completing its flight.
    /// </summary>
    private void DestroyAirstrike()
    {
        Destroy(gameObject);
    }

    #endregion
}