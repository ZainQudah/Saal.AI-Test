using UnityEngine;

/// <summary>
/// Represents a tank that moves towards a target and fires at it.
/// </summary>
public class Tank : Threat
{
    #region Serialized Fields (Unity Inspector)

    [Header("Tank Settings")] [Tooltip("The distance from the target at which the tank stops moving.")] [SerializeField]
    private float stopDistance = 20f;

    [Tooltip("The delay before the tank fires after reaching its target.")] [SerializeField]
    private float fireDelay = 3f;

    [Tooltip("The weapon's transform used for aiming.")] [SerializeField]
    private Transform weapon;

    [Tooltip("The collider for the tank's body, used to detect collisions or hits.")] [SerializeField]
    private Collider bodyCollider;

    [Tooltip("The transform point where the weapon fires projectiles.")] [SerializeField]
    private Transform firingPoint;

    [Header("FX Settings")] [Tooltip("Particle system prefab spawned when the tank fires its weapon.")] [SerializeField]
    private GameObject particleSystem2;

    #endregion

    #region Private Fields

    private Transform _target; // The target the tank moves towards (e.g., the player's base).
    private bool hasFired = false; // Tracks whether the tank has already fired.
    private Transform player; // Reference to the player's transform.

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Called every frame to update the tank's behavior.
    /// </summary>
    private void Update()
    {
        if (_target != null && !hasFired)
        {
            MoveToTarget();
        }
    }

    #endregion

    #region Tank Functionality

    /// <summary>
    /// Initializes the tank and sets its target.
    /// </summary>
    /// <param name="targetPosition">Position of the target to move towards.</param>
    public override void Launch(Vector3 targetPosition)
    {
        _target = GameObject.FindGameObjectWithTag("LandTarget").transform;
    }

    /// <summary>
    /// Moves the tank towards the target until it reaches the stopping distance.
    /// </summary>
    private void MoveToTarget()
    {
        // Calculate the distance to the target.
        float distanceToTarget = Vector3.Distance(transform.position, _target.position);

        if (distanceToTarget > stopDistance)
        {
            // Move towards the target.
            Vector3 direction = (_target.position - transform.position).normalized;
            transform.position += direction * (speed * Time.deltaTime);

            // Smoothly rotate to face the target.
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
        else if (!hasFired)
        {
            // Start the aiming phase if the target is within the stopping distance.
            AimAndFire();
        }
    }

    /// <summary>
    /// Aims the weapon at the player and prepares to fire.
    /// </summary>
    private void AimAndFire()
    {
        if (_target != null)
        {
            // Find the player's position and rotate the weapon to face it.
            player = GameObject.FindGameObjectWithTag("Player").transform;
            Vector3 direction = (player.position - weapon.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            weapon.rotation = Quaternion.Slerp(weapon.rotation, lookRotation, Time.deltaTime);

            // Schedule the firing phase after a delay.
            if (!hasFired)
            {
                Invoke(nameof(Fire), fireDelay);
                hasFired = true;
            }
        }
    }

    /// <summary>
    /// Fires the weapon at the target and triggers related effects.
    /// </summary>
    private void Fire()
    {
        // Disable the tank's collider and instantiate visual effects.
        bodyCollider.enabled = false;
        Instantiate(particleSystem2, firingPoint.position, firingPoint.rotation);

        // Trigger explosion effects at the player's position.
        Instantiate(particleSystem, player.position, player.rotation);

        // Play related sound effects.
        SoundManager.Instance.ExplosionSfx();
        SoundManager.Instance.NegativeCue();
        ScreenEffects.Instance.TriggerRedFlash();


        // Schedule the removal of the tank after firing.
        Invoke(nameof(RemoveTank), 2f);
    }

    /// <summary>
    /// Removes the tank from the scene after it has completed firing.
    /// </summary>
    private void RemoveTank()
    {
        Destroy(gameObject);
    }

    #endregion
}