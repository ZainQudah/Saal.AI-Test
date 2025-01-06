using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the spawning and launching of threats in the game. 
/// Handles proper spawning logic, sound effects, and scheduling.
/// </summary>
public class ThreatManager : MonoBehaviour
{
    #region Serialized Fields (Unity Inspector)

    [Header("Threat Configuration")]
    [Tooltip("List of all available threat prefabs that can be spawned.")]
    [SerializeField]
    private List<Threat> threatPrefabs;

    [Tooltip("Total number of threats to spawn in the wave.")] [SerializeField, Min(1)]
    private int totalThreats = 5;

    [Header("Spawn Points")] [Tooltip("List of generic spawn points for threats.")] [SerializeField]
    private List<Transform> spawnPoints;

    [Tooltip("Spawn point for Submarines.")] [SerializeField]
    private Transform submarineSpawnPoint;

    [Tooltip("Spawn point for Tanks.")] [SerializeField]
    private Transform tankSpawnPoint;

    [Header("Final Destination")]
    [Tooltip("The target point threats will try to reach (e.g., player's base).")]
    [SerializeField]
    private Transform targetPoint;

    #endregion

    #region Private Fields

    private List<Threat> availableThreats; // Remaining threats available for spawning.
    private Transform spawnPoint; // Specific spawn point for the chosen threat.

    #endregion

    #region MonoBehaviour Events

    /// <summary>
    /// Initializes the threat manager and begins threat spawning.
    /// </summary>
    private void Start()
    {
        // Clone the original list of threats to manage them dynamically during play.
        availableThreats = new List<Threat>(threatPrefabs);

        // Start managing threats on a timed schedule.
        StartCoroutine(ManageThreats());
    }

    #endregion

    #region Threat Management

    /// <summary>
    /// Manages the spawning and launching of threats in timed intervals.
    /// Spawns a specified number of threats and handles their behavior.
    /// </summary>
    private IEnumerator ManageThreats()
    {
        for (int i = 0; i < totalThreats; i++)
        {
            // Stop spawning if no threats are available.
            if (availableThreats.Count == 0)
                break;

            // Choose a random threat from the available list.
            int index = Random.Range(0, availableThreats.Count);
            Threat chosenThreat = availableThreats[index];

            // Determine the appropriate spawn point based on the type of threat.
            spawnPoint = DetermineSpawnPoint(chosenThreat);

            // Play an appropriate sound based on the type of threat.
            PlayThreatSpawnSound(chosenThreat);

            // Instantiate and launch the threat.
            Threat threatInstance = Instantiate(chosenThreat, spawnPoint.position, spawnPoint.rotation);
            threatInstance.Launch(targetPoint.position);

            // Remove the spawned threat from the available list.
            availableThreats.RemoveAt(index);

            // Wait until the threat is averted or reaches the target.
            while (threatInstance != null)
            {
                yield return null; // Wait for the next frame.
            }

            // Add a delay before spawning the next threat.
            yield return new WaitForSeconds(2f);
        }

        // All threats have been spawned; notify the GameManager.
        GameManager.Instance.GameOver();
    }

    /// <summary>
    /// Determines the appropriate spawn point for the given threat type.
    /// </summary>
    /// <param name="threat">The threat instance to spawn.</param>
    /// <returns>The determined spawn point as a Transform.</returns>
    private Transform DetermineSpawnPoint(Threat threat)
    {
        if (threat.TryGetComponent(out Submarine _))
        {
            return submarineSpawnPoint; // Use the submarine-specific spawn point.
        }
        else if (threat.TryGetComponent(out Tank _))
        {
            return tankSpawnPoint; // Use the tank-specific spawn point.
        }
        else
        {
            // Use a random generic spawn point for other types of threats.
            return spawnPoints[Random.Range(0, spawnPoints.Count)];
        }
    }

    /// <summary>
    /// Plays the appropriate spawn sound based on the type of threat.
    /// </summary>
    /// <param name="threat">The threat instance to play a sound for.</param>
    private void PlayThreatSpawnSound(Threat threat)
    {
        switch (threat)
        {
            case Airstrike _:
                SoundManager.Instance.PlaySoundOneShot(SoundManager.Instance.airStrike);
                break;

            case Rocket _:
                SoundManager.Instance.PlaySoundOneShot(SoundManager.Instance.artilleryStrike);
                break;

            default:
                // Play a random alert sound for other kinds of threats.
                int randomSound = Random.Range(0, 2);
                if (randomSound == 0)
                    SoundManager.Instance.PlaySoundOneShot(SoundManager.Instance.enemyMove);
                else
                    SoundManager.Instance.PlaySoundOneShot(SoundManager.Instance.hostilesInbound);
                break;
        }
    }

    #endregion
}