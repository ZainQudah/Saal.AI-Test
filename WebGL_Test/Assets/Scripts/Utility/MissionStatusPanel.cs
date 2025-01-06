using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the display of the mission status panel, including success/fail indicators
/// and star ratings based on the player's score.
/// </summary>
public class MissionStatusPanel : MonoBehaviour
{
    #region Serialized Fields (Unity Inspector)

    [Header("Mission Status UI")] [Tooltip("GameObject that displays the mission success message.")] [SerializeField]
    private GameObject MissionSuccess;

    [Tooltip("GameObject that displays the mission failure message.")] [SerializeField]
    private GameObject MissionFail;

    [Header("Star Rating")] [Tooltip("Array of Image components representing the stars in the UI.")] [SerializeField]
    private Image[] stars;

    [Tooltip("Opacity for enabled stars (fully visible).")] [SerializeField]
    private float enabledOpacity = 1f;

    [Tooltip("Opacity for disabled stars (faded appearance).")] [SerializeField]
    private float disabledOpacity = 0.25f;

    #endregion

    #region MonoBehaviour Events

    /// <summary>
    /// Called when the GameObject becomes active. Updates mission status and star display.
    /// </summary>
    private void OnEnable()
    {
        // Show success or failure message based on the player's score.
        UpdateMissionStatus(GameManager.Instance.playerScore);

        // Update the stars display according to the player's score.
        UpdateStarDisplay(GameManager.Instance.playerScore);
    }

    #endregion

    #region UI Update Methods

    /// <summary>
    /// Updates the mission success or failure display based on the player's score.
    /// </summary>
    /// <param name="playerScore">The player's score, used to determine mission success.</param>
    private void UpdateMissionStatus(int playerScore)
    {
        if (playerScore >= 3)
        {
            MissionSuccess.SetActive(true);
            MissionFail.SetActive(false);
        }
        else
        {
            MissionFail.SetActive(true);
            MissionSuccess.SetActive(false);
        }
    }

    /// <summary>
    /// Updates the display of the stars in the UI by enabling stars based on the player's score.
    /// </summary>
    /// <param name="playerScore">The number of stars to display as enabled.</param>
    private void UpdateStarDisplay(int playerScore)
    {
        // Ensure the player's score is clamped between 0 and the number of stars.
        playerScore = Mathf.Clamp(playerScore, 0, stars.Length);

        for (int i = 0; i < stars.Length; i++)
        {
            // Enable stars up to the player's score and disable the rest.
            if (i < playerScore)
            {
                SetStarOpacity(stars[i], enabledOpacity); // Fully visible star.
            }
            else
            {
                SetStarOpacity(stars[i], disabledOpacity); // Faded star.
            }
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Sets the opacity of a star UI element by modifying its alpha value.
    /// </summary>
    /// <param name="star">The star Image component to modify.</param>
    /// <param name="opacity">The desired opacity level (between 0 and 1).</param>
    private void SetStarOpacity(Image star, float opacity)
    {
        if (star != null)
        {
            Color color = star.color; // Get the current color of the star.
            color.a = opacity; // Set the alpha value to adjust opacity.
            star.color = color; // Apply the updated color back to the star.
        }
    }

    #endregion
}