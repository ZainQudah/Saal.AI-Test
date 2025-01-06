using UnityEngine;

/// <summary>
/// Base class for defense mechanisms. It provides common behavior for deployment,
/// area-of-effect visualization, and related defense mechanics.
/// </summary>
public abstract class Defense : MonoBehaviour
{
    #region Serialized Fields (Unity Inspector)

    [Header("Defense Settings")] [Tooltip("Radius of the defense area of effect.")]
    public float radius = 5f;

    [Tooltip("LineRenderer used for visualizing the area of effect (AoE).")] [SerializeField]
    protected LineRenderer indicator;

    [Tooltip("Particle system used for visual or gameplay effects.")] [SerializeField]
    public GameObject particleSystem;
    
    private string poolKey;


    #endregion

    #region Public Methods

    /// <summary>
    /// Deploys the defense object at the specified world position.
    /// </summary>
    /// <param name="position">The target deployment position.</param>
    public virtual void Deploy(Vector3 position)
    {
        // Move the defense object to the specified position.
        transform.position = position;
        /*
        ShowIndicator(position, radius);
        */

    }

    /// <summary>
    /// Displays a circular indicator to visualize the defense's area of effect (AoE).
    /// </summary>
    /// <param name="deployPosition">The central position of the AoE.</param>
    /// <param name="radius">The radius of the AoE to visualize.</param>
    public virtual void ShowIndicator(Vector3 deployPosition, float radius)
    {
        if (indicator == null) return; // Exit if no LineRenderer is assigned.

        // Set the number of points in the circle (resolution).
        indicator.positionCount = 50 + 1;

        // Draw a circular pattern around the deploy position.
        float angle = 0f;
        for (int i = 0; i < indicator.positionCount; i++)
        {
            // Calculate x and z positions for the current angle.
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

            // Set the point on the LineRenderer.
            indicator.SetPosition(i, new Vector3(x, 0, z) + deployPosition);

            // Increment the angle for the next point.
            angle += 360f / (indicator.positionCount - 1);
        }

        // Close the circle and enable the LineRenderer.
        indicator.loop = true;
        indicator.enabled = true;
    }

    /// <summary>
    /// Hides the area-of-effect indicator by disabling the LineRenderer.
    /// </summary>
    public virtual void HideIndicator()
    {
        if (indicator != null)
        {
            indicator.enabled = false;
        }
    }
    public void InitializePoolKey(string key)
    {
        poolKey = key;
    }

    public void ReturnToPool()
    {
        HideIndicator();
        PoolManager.Instance.ReturnToPool(poolKey, gameObject);
    }

    #endregion
}