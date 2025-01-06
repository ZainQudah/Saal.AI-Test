using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages object pooling for optimized instantiation and reuse of GameObjects.
/// </summary>
public class PoolManager : MonoBehaviour
{
    #region Singleton

    /// <summary>
    /// The singleton instance of the PoolManager.
    /// </summary>
    public static PoolManager Instance { get; private set; }

    private void Awake()
    {
        // Ensure a single instance of PoolManager exists
        if (Instance == null)
        {
            Instance = this;
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    #endregion

    #region Variables

    [Header("Pool Dictionary")]
    [Tooltip("Dictionary to manage object pools by key.")]
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    #endregion

    #region Pool Management

    /// <summary>
    /// Creates a new object pool for the specified key and prefab.
    /// </summary>
    /// <param name="key">The unique key for the object pool.</param>
    /// <param name="prefab">The prefab to instantiate for the pool.</param>
    /// <param name="initialSize">The initial number of objects in the pool.</param>
    public void CreatePool(string key, GameObject prefab, int initialSize)
    {
        if (!poolDictionary.ContainsKey(key))
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false); // Deactivate the object
                objectPool.Enqueue(obj); // Add to the pool
            }

            poolDictionary[key] = objectPool;
        }
        else
        {
            Debug.LogWarning($"Pool with key {key} already exists.");
        }
    }

    /// <summary>
    /// Retrieves an object from the specified pool.
    /// </summary>
    /// <param name="key">The key for the object pool.</param>
    /// <param name="position">The position to place the object.</param>
    /// <param name="rotation">The rotation to apply to the object.</param>
    /// <returns>The GameObject from the pool, or null if none are available.</returns>
    public GameObject GetFromPool(string key, Vector3 position, Quaternion rotation)
    {
        if (poolDictionary.ContainsKey(key) && poolDictionary[key].Count > 0)
        {
            GameObject obj = poolDictionary[key].Dequeue();
            obj.SetActive(true);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            return obj;
        }

        Debug.LogWarning($"No objects available in pool: {key}");
        return null;
    }

    /// <summary>
    /// Returns an object to the specified pool.
    /// </summary>
    /// <param name="key">The key for the object pool.</param>
    /// <param name="obj">The GameObject to return to the pool.</param>
    public void ReturnToPool(string key, GameObject obj)
    {
        if (poolDictionary.ContainsKey(key))
        {
            obj.SetActive(false);
            poolDictionary[key].Enqueue(obj); // Add the object back to the pool
        }
        else
        {
            Debug.LogWarning($"Pool with key {key} does not exist. Destroying object.");
            Destroy(obj); // Fallback: destroy the object if the pool doesn't exist
        }
    }

    #endregion
}
