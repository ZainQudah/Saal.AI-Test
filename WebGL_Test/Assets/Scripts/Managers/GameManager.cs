using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles core gameplay functionality, including scene management,
/// player score tracking, and UI updates for game over and control panels.
/// Implements a singleton pattern for persistent access between scenes.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton Implementation

    /// <summary>
    /// Singleton instance of the GameManager for global access.
    /// </summary>
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Ensure that there's only one instance of the GameManager.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances.
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes.
        }
    }

    #endregion

    #region Serialized Fields (Unity Inspector)

    [Header("UI Elements")] [Tooltip("Control Panel GameObject used during gameplay.")] [SerializeField]
    private GameObject controlPanel;

    [Tooltip("Game Over UI GameObject displayed after mission end.")] [SerializeField]
    private GameObject gameOverUI;

    #endregion

    #region Private Fields

    private Button level2; // Reference to the button for Level 2 selection.

    #endregion
    
    #region Public Fields
    
    public int playerScore = 0; // Current player's score.
    
    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Subscribes to the sceneLoaded event when this GameObject is enabled.
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Unsubscribes from the sceneLoaded event when this GameObject is disabled.
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Handles actions to be executed each time a new scene is loaded.
    /// </summary>
    /// <param name="scene">The loaded scene.</param>
    /// <param name="mode">The scene loading mode.</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            SetupMainMenu();
        }
        else if (scene.name == "07 - Wild West")
        {
            SetupGameplay();
        }
    }

    #endregion

    #region Scene Setup Methods

    /// <summary>
    /// Sets up the Main Menu by enabling or disabling Level 2 based on player score.
    /// </summary>
    private void SetupMainMenu()
    {
        // Find the Level 2 button in the Main Menu scene.
        level2 = GameObject.FindGameObjectWithTag("Level2").GetComponent<Button>();

        // Enable or disable the Level 2 button based on the player's score.
        level2.interactable = (playerScore >= 3);
    }

    /// <summary>
    /// Initializes gameplay for the "07 - Wild West" level by resetting player state and UI.
    /// </summary>
    private void SetupGameplay()
    {
        // Reset the player's score at the start of the gameplay level.
        playerScore = 0;

        // Find and configure the necessary UI elements.
        controlPanel = GameObject.FindGameObjectWithTag("Controls");
        gameOverUI = GameObject.FindGameObjectWithTag("MissionStatus");
        gameOverUI.SetActive(false);
    }

    #endregion

    #region Game Actions

    /// <summary>
    /// Increases the player's score and plays a positive sound cue.
    /// Called when a threat is successfully averted.
    /// </summary>
    public void ThreatAverted()
    {
        Debug.Log(playerScore);
        
        playerScore++;
        SoundManager.Instance.PositiveCue(); // Play a feedback sound for the successful event.
        ScreenEffects.Instance.TriggerGreenFlash();
        
        Debug.Log(playerScore + " after");
        
    }

    /// <summary>
    /// Triggers the end of the game sequence, displaying the Game Over UI
    /// and playing appropriate sounds based on the player's score.
    /// </summary>
    public void GameOver()
    {
        // Play appropriate sounds based on the player's performance.
        if (playerScore >= 3)
        {
            SoundManager.Instance.PlaySoundScheduled(SoundManager.Instance.objectiveSecured);
            SoundManager.Instance.PlaySoundScheduled(SoundManager.Instance.missionAccomplished);
        }
        else
        {
            SoundManager.Instance.PlaySoundOneShot(SoundManager.Instance.objectiveLost);
        }

        // Update UI to display Game Over.
        controlPanel.SetActive(false);
        gameOverUI.SetActive(true);
    }

    #endregion
}