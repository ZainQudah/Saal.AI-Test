using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;

/// <summary>
/// Singleton class that handles scene loading across the game.
/// Ensures there is only one persistent instance for managing scenes.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    #region Singleton Implementation

    /// <summary>
    /// Global instance of the SceneLoader for access across scenes.
    /// </summary>
    public static SceneLoader Instance { get; private set; }

    /// <summary>
    /// Ensures that this object follows the Singleton pattern.
    /// If there is already an instance, the duplicate is destroyed.
    /// Otherwise, the object is marked as persistent.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate SceneLoader instances.
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist this SceneLoader across scenes.
        }
    }

    #endregion

    #region Scene Management

    /// <summary>
    /// Loads a scene by its name.
    /// Should be used for transitioning between game levels or menus.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void LoadScene(string sceneName)
    {
        if (sceneName == "MainMenu" || sceneName == "Level2Scene")
            SceneManager.LoadScene(sceneName); // Transition to the specified scene.
        else
        {
            Addressables.LoadSceneAsync(sceneName);
        }
    }

    #endregion
}