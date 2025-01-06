using UnityEngine;

/// <summary>
/// Base class for all threats that move towards a target.
/// Handles common movement functionality and provides hooks for derived classes.
/// </summary>
public abstract class Threat : MonoBehaviour
{
    #region Serialized Fields (Unity Inspector)

    [Header("Threat Settings")] [Tooltip("The movement speed of the threat.")] [SerializeField]
    public float speed = 5f;

    [Tooltip("Particle system prefab to spawn upon impact.")] [SerializeField]
    public GameObject particleSystem;

    #endregion

    #region Protected Fields

    // Position of the target this threat will move towards.
    protected Vector3 target;

    #endregion

    #region Public Methods

    /// <summary>
    /// Launches the threat towards a specific target position.
    /// </summary>
    /// <param name="targetPosition">The position of the target.</param>
    public virtual void Launch(Vector3 targetPosition)
    {
        target = targetPosition;
       
    }

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Updates the threat's position each frame.
    /// Moves it towards the assigned target position.
    /// </summary>
    protected virtual void Update()
    {
        // Move directly towards the target position.
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        
    }

    #endregion

}